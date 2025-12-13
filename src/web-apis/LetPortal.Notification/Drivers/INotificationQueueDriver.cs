using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Notification.Entities;

namespace LetPortal.Notification.Drivers
{
    public interface INotificationQueueDriver
    {
        string Driver { get; }

        Task StartAsync();

        Task PushAsync(IncomingNotificationMessage message);

        Task SubcribeAsync(Func<IncomingNotificationMessage, Task> proceed, CancellationToken cancellationToken);
    }
}
