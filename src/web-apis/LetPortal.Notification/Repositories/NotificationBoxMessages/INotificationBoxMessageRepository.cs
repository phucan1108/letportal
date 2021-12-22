using LetPortal.Core.Notifications;
using LetPortal.Core.Persistences;
using LetPortal.Notification.Entities;

namespace LetPortal.Notification.Repositories.NotificationBoxMessages
{
    public interface INotificationBoxMessageRepository : IGenericRepository<NotificationBoxMessage>
    {
        Task<NotificationBoxMessage> TakeLastOfGroup(string subcriberId, string messageGroupId);

        Task<IEnumerable<NotificationBoxMessage>> TakeLastAsync
            (string subcriberId,
             string messageGroupId,
             long lastVisitedTs,
             int numberOfMessages,
             NotificationType[] notificationTypes);

        Task<int> CheckUnreadMessagesAsync(
            string subcriberId, 
            string messageGroupId,
            long lastVisitedTs);
    }
}
