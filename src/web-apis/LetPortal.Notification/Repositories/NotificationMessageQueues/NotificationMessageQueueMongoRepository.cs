using LetPortal.Core.Persistences;
using LetPortal.Notification.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LetPortal.Notification.Repositories.NotificationMessageQueues
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

        public async Task Listen(Func<NotificationMessageQueue, Task> proceed, CancellationToken cancellationToken)
        {
            DateTime lastInsertDate = DateTime.UtcNow;

            var options = new FindOptions<NotificationMessageQueue>
            {
                // Our cursor is a tailable cursor and informs the server to await
                CursorType = CursorType.TailableAwait
            };
            while (!cancellationToken.IsCancellationRequested)
            {
                using (var cursor = await Collection.FindAsync(a => a.SentDate >= lastInsertDate, options))
                {
                    // This callback will get invoked with each new document found
                    await cursor.ForEachAsync(async document =>
                    {
                        // Set the last value we saw 
                        lastInsertDate = document.SentDate;

                        await proceed.Invoke(document);
                    });
                }
            }
        }
    }
}
