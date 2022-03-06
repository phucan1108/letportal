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

        Task Push(IncomingNotificationMessage message);

        Task Subcribe(Func<IncomingNotificationMessage, Task> proceed, CancellationToken cancellationToken);
    }
}
