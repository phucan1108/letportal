using LetPortal.Core.Persistences;
using LetPortal.Notification.Entities;

namespace LetPortal.Notification.Repositories.MessageGroups
{
    public class MessageGroupMongoRepository : MongoGenericRepository<MessageGroup>, IMessageGroupRepository
    {
        public MessageGroupMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }

        public async Task UpdateLastClicked(string subcriberId, string messageGroupId, long lastClickedTs)
        {
            var messageGroup = await FindAsync(a => a.SubcriberId == subcriberId && a.Id == messageGroupId);
            messageGroup.LastVisitedTs = lastClickedTs;
            await UpdateAsync(messageGroup.Id, messageGroup);
        }
    }
}
