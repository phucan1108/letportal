using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Identity.Entities;
using MongoDB.Driver;

namespace LetPortal.Identity.Repositories.Identity
{
    public class UserSessionMongoRepository : MongoGenericRepository<UserSession>, IUserSessionRepository
    {
        public UserSessionMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }

        public async Task LogUserActivityAsync(string userSessionId, UserActivity userActivity)
        {
            var updateBuilder = Builders<UserSession>.Update;
            var updateDefinition = updateBuilder.Push(a => a.UserActivities, userActivity);
            await Collection.FindOneAndUpdateAsync(a => a.Id == userSessionId, updateDefinition);
        }
    }
}
