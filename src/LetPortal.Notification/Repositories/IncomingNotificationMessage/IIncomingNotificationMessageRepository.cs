using LetPortal.Core.Persistences;
using LetPortal.Notification.Entities;

namespace LetPortal.Notification.Repositories.NotificationMessageQueues
{
    public interface IIncomingNotificationMessageRepository : IGenericRepository<IncomingNotificationMessage>
    {
        Task Listen(Func<IncomingNotificationMessage, Task> proceed, CancellationToken cancellationToken);
    }
}
