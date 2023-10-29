using LetPortal.Notification.Models.RealTimes;

namespace LetPortal.Notification.Hubs
{
    public interface INotificationHubClient
    {
        Task Push(OnlineNotificationMessage notificationMessage);

        Task PushNewGroup(OnlineMessageGroup onlineMessageGroup);

        Task MarkRead(string notificationMessageId);
    }
}
