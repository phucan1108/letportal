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
    }
}
