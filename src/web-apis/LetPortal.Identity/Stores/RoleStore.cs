using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using LetPortal.Core.Utils;
using LetPortal.Identity.Entities;
using LetPortal.Identity.Repositories.Identity;
using Microsoft.AspNetCore.Identity;

namespace LetPortal.Identity.Stores
{
    public class RoleStore : IRoleStore<Role>, IRoleClaimStore<Role>, IDisposable
    {
        private readonly IRoleRepository _roleRepository;

        public RoleStore(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public Task AddClaimAsync(Role role, Claim claim, CancellationToken cancellationToken = default)
        {
            var baseClaim = claim.ToBaseClaim();
            if (role.Claims.Any(a => a.ClaimType == baseClaim.ClaimType && a.ClaimValue == baseClaim.ClaimValue))
            {
                return Task.CompletedTask;
            }
            role.Claims.Add(claim.ToBaseClaim());
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
        {
            role.Id = !string.IsNullOrEmpty(role.Id) ? role.Id : DataUtil.GenerateUniqueId();
            await _roleRepository.AddAsync(role);
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
        {
            await _roleRepository.DeleteAsync(role.Id);
            return IdentityResult.Success;
        }

        public async Task<Role> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            return await _roleRepository.GetOneAsync(roleId);
        }

        public Task<Role> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            return Task.FromResult(_roleRepository.GetAsQueryable().FirstOrDefault(a => a.NormalizedName == normalizedRoleName));
        }

        public Task<IList<Claim>> GetClaimsAsync(Role role, CancellationToken cancellationToken = default)
        {
            return Task.FromResult((IList<Claim>)role.Claims.Select(a => a.ToClaim()).ToList());
        }

        public Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.NormalizedName);
        }

        public Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Id);
        }

        public Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task RemoveClaimAsync(Role role, Claim claim, CancellationToken cancellationToken = default)
        {
            var foundClaim = role.Claims.FirstOrDefault(a => a.ClaimType == claim.Type && a.ClaimValue == claim.Value);
            if (foundClaim != null)
            {
                var foundIndex = role.Claims.IndexOf(foundClaim);
                role.Claims.RemoveAt(foundIndex);
            }

            return Task.CompletedTask;
        }

        public Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            await _roleRepository.UpdateAsync(role.Id, role);
            return IdentityResult.Success;
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _roleRepository.Dispose();
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
