using LetPortal.Core.Persistences;
using LetPortal.Notification.Entities;
using MongoDB.Driver;

namespace LetPortal.Notification.Repositories.Subcribers
{
    public class SubcriberMongoRepository : MongoGenericRepository<Subcriber>, ISubscriberRepository
    {
        public SubcriberMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }

        public async Task<IEnumerable<Subcriber>> GetByRoles(string[] roles)
        {
            var filter = Builders<Subcriber>.Filter.In("roles", roles);
            var result = await Collection.FindAsync(filter);

            return result.Current;
        }

        public async Task UpdateLastClicked(string subcriberId, long lastClickedTs)
        {
            var subcriber = await GetOneAsync(subcriberId);
            subcriber.LastClickedBoxTs = lastClickedTs;
            await UpdateAsync(subcriberId, subcriber);
        }
    }
}
