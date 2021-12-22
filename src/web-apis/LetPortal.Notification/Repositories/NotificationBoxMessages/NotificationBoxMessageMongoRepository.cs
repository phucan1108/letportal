using LetPortal.Core.Notifications;
using LetPortal.Core.Persistences;
using LetPortal.Notification.Entities;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace LetPortal.Notification.Repositories.NotificationBoxMessages
{
    public class NotificationBoxMessageMongoRepository : MongoGenericRepository<NotificationBoxMessage>, INotificationBoxMessageRepository
    {
        public NotificationBoxMessageMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
            ScanIndexs();
        }

        public async Task<int> CheckUnreadMessagesAsync(
            string subcriberId,
            string messageGroupId,
            long lastVisitedTs)
        {
            return await Collection
                    .AsQueryable()
                    .Where(a => a.MessageGroupId == messageGroupId && a.ReceivedDateTs > lastVisitedTs)
                    .CountAsync();
        }

        public async Task<IEnumerable<NotificationBoxMessage>> TakeLastAsync
            (string subcriberId,
             string messageGroupId,
             long lastVisitedTs,
             int numberOfMessages,
             NotificationType[] notificationTypes)
        {
            if (notificationTypes != null && notificationTypes.Length > 0)
            {
                var filterSubcriberId = Builders<NotificationBoxMessage>.Filter.Eq(a => a.SubcriberId, subcriberId);
                var filterMessageGroupId = Builders<NotificationBoxMessage>.Filter.Eq(a => a.MessageGroupId, messageGroupId);
                var filterNotificationTypes = Builders<NotificationBoxMessage>.Filter.In(a => a.Type, notificationTypes);
                var filterLastFetchedTs = Builders<NotificationBoxMessage>.Filter.Lt(a => a.ReceivedDateTs, lastVisitedTs);
                var andCombinedFilter = Builders<NotificationBoxMessage>.Filter
                        .And(
                            filterSubcriberId, 
                            filterMessageGroupId,
                            filterLastFetchedTs,
                            filterNotificationTypes);

                var sort = Builders<NotificationBoxMessage>.Sort.Descending(a => a.ReceivedDateTs);

                return Collection.Find(andCombinedFilter).Sort(sort).Limit(numberOfMessages).ToEnumerable().OrderBy(a => a.ReceivedDateTs);                
            }
            else
            {
                return (await Collection
                    .AsQueryable()
                    .Where(a => a.MessageGroupId == messageGroupId && a.ReceivedDateTs < lastVisitedTs)
                    .OrderByDescending(b => b.ReceivedDateTs)
                    .Take(numberOfMessages) // Keep the naite order asc
                    .ToListAsync())
                    .OrderBy(c => c.ReceivedDateTs);
            }
        }

        public async Task<NotificationBoxMessage> TakeLastOfGroup(string subcriberId, string messageGroupId)
        {
            var last = await Collection.AsQueryable().Where(a => a.MessageGroupId == messageGroupId && a.SubcriberId == subcriberId).OrderByDescending(b => b.ReceivedDateTs).Take(1).ToListAsync();
            return last?.First();
        }
    }
}
