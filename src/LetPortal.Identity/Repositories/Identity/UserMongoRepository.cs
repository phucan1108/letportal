using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Identity.Entities;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace LetPortal.Identity.Repositories.Identity
{
    public class UserMongoRepository : MongoGenericRepository<User>, IUserRepository
    {
        public UserMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }

        public async Task<User> FindByNormalizedUsername(string normilizedName)
        {
            return await Collection.AsQueryable().FirstOrDefaultAsync(a => a.NormalizedUserName == normilizedName);
        }
    }
}
