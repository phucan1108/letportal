using LetPortal.Core.Persistences;
using LetPortal.Notification.Entities;

namespace LetPortal.Notification.Repositories.NotificationMessageQueues
{
    public interface INotificationMessageQueueRepository : IGenericRepository<NotificationMessageQueue>
    {
        Task Listen(Func<NotificationMessageQueue, Task> proceed, CancellationToken cancellationToken);
    }
}
