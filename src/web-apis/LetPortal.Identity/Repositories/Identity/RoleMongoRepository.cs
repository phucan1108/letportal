using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Identity.Entities;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace LetPortal.Identity.Repositories.Identity
{
    public class RoleMongoRepository : MongoGenericRepository<Role>, IRoleRepository
    {
        public RoleMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }

        public async Task<Dictionary<string, List<BaseClaim>>> GetBaseClaims(string[] roles)
        {
            var dic = new Dictionary<string, List<BaseClaim>>();

            var foundRoles = await Collection.AsQueryable().Where(a => roles.Contains(a.Name)).ToListAsync();

            var kvps = foundRoles.Select(a => new KeyValuePair<string, List<BaseClaim>>(a.Name, a.Claims)).ToList();
            foreach (var kv in kvps)
            {
                dic.Add(kv.Key, kv.Value);
            }

            return dic;
        }

        public async Task<Role> GetByNameAsync(string name)
        {
            return await Collection.AsQueryable().FirstOrDefaultAsync(a => a.Name == name);
        }
    }
}
