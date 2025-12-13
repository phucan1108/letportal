using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Core.Security;
using LetPortal.Core.Versions;
using LetPortal.Identity;
using LetPortal.Identity.Entities;
using LetPortal.Identity.Repositories.Identity;
using LetPortal.Portal.Constants;

namespace LetPortal.Versions.Identity
{
    public class Identity_0_8_1 : IIdentityVersion
    {
        private readonly IRoleRepository _roleRepository;

        public Identity_0_8_1(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public string VersionNumber => "0.8.1";

        public Task Downgrade(IVersionContext versionContext)
        {
            return Task.CompletedTask;
        }

        public async Task Upgrade(IVersionContext versionContext)
        {
            var adminRole = await _roleRepository.GetByNameAsync(Roles.Admin);
            var developerRole = await _roleRepository.GetByNameAsync(Roles.Developer);
            adminRole.Claims.AddRange(
                StandardClaims.GenerateClaimsByPages(
                    new string[]{
                        "app-installation", 
                        "app-package" }));
            developerRole.Claims.AddRange(
               StandardClaims.GenerateClaimsByPages(
                   new string[]{
                        "app-installation",
                        "app-package" }));

            await _roleRepository.UpdateAsync(adminRole.Id, adminRole);
            await _roleRepository.UpdateAsync(developerRole.Id, developerRole);
        }
    }
}
