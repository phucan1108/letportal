using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Notification.Models.RealTimes;

namespace LetPortal.Notification.Hubs
{
    public interface INotificationHubClient
    {
        Task Push(OnlineNotificationMessage notificationMessage); 

        Task MarkRead(string notificationMessageId);
    }
}
