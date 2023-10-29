using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using LetPortal.Notification.Entities;
using LetPortal.Notification.Models;

namespace LetPortal.Notification.Channels
{
    public interface INotificationChannel<T> where T : IncomingMessage
    {
        NotificationScope Scope { get; }

        Channel<T> GetChannel();
    }
}
