using LetPortal.Notification.Models;
using LetPortal.Notification.Models.RealTimes;

namespace LetPortal.Notification.Services
{
    public interface ISubcriberService
    {
        Task<OnlineSubcriber> Subcribe(string userId, string userName, string[] roles);

        Task MarkRead(string subcriberId, string notificationId);

        Task Receive(GlobalIncomingMessage globalMessage, Func<OnlineNotificationMessage, Task> postAction);

        Task Receive(RoleIncomingMessage roleMessage, Func<OnlineNotificationMessage, Task> postAction);

        Task Receive(IndividualIncomingMessage incomingMessage, Func<OnlineNotificationMessage, Task> postAction);

        Task ClickOnNotificationBox(string subcriberId, long clickedTicks);
    }
}
