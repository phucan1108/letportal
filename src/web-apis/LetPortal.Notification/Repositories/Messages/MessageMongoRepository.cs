using LetPortal.Core.Persistences;
using LetPortal.Notification.Entities;

namespace LetPortal.Notification.Repositories.Messages
{
    public class MessageMongoRepository : MongoGenericRepository<Message>, IMessageRepository
    {
        public MessageMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }
    }
}
