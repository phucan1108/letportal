using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Identity.Entities;

namespace LetPortal.Identity.Repositories.Identity
{
    public class RoleEFRepository : EFGenericRepository<Role>, IRoleRepository
    {
        private readonly IdentityDbContext _context;

        public RoleEFRepository(IdentityDbContext context)
            : base(context)
        {
            _context = context;
        }

        public Task<Dictionary<string, List<BaseClaim>>> GetBaseClaims(string[] roles)
        {
            var dic = new Dictionary<string, List<BaseClaim>>();

            var foundRoles = _context.Roles.Where(a => roles.Contains(a.Name)).ToList();

            var kvps = foundRoles.Select(a => new KeyValuePair<string, List<BaseClaim>>(a.Name, a.Claims)).ToList();
            foreach (var kv in kvps)
            {
                dic.Add(kv.Key, kv.Value);
            }

            return Task.FromResult(dic);
        }

        public Task<Role> GetByNameAsync(string name)
        {
            return Task.FromResult(_context.Roles.First(a => a.Name == name));
        }
    }
}
