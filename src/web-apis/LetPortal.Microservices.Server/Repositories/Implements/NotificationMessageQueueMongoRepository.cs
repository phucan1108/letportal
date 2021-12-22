using LetPortal.Core.Persistences;
using LetPortal.Microservices.Server.Entities;
using LetPortal.Microservices.Server.Repositories.Abstractions;

namespace LetPortal.Microservices.Server.Repositories.Implements
{
    public class NotificationMessageQueueMongoRepository : MongoGenericRepository<NotificationMessageQueue>, INotificationMessageQueueRepository
    {
        public NotificationMessageQueueMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
            if (!IsExisted())
            {
                SetCappedCollection();
            }            
        }
    }
}
