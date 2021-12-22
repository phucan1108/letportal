using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Core.Notifications;
using LetPortal.Notification.Entities;

namespace LetPortal.Notification.Models.RealTimes
{
    public class OnlineNotificationMessage
    {
        public string NotificationBoxId { get; set; }

        public string SubcriberId { get; set; }

        public string MessageGroupId { get; set; }

        public string MessageId { get; set; }

        public string MessageGroupName { get; set; }

        public NotificationType Type { get; set; }

        public string ShortMessage { get; set; }

        public bool IsDirty { get; set; }

        public DateTime ReceivedDate { get; set; }

        public DateTime ClickedDate { get; set; }

        public long ReceivedDateTs { get; set; }
    }
}
