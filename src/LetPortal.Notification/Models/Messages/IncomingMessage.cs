using LetPortal.Core.Notifications;
using LetPortal.Notification.Entities;

namespace LetPortal.Notification.Models
{
    public abstract class IncomingMessage
    {
        public string Code { get; set; }

        public NotificationType Type { get; set; }

        public string Content { get; set; }

        public string Source { get; set; }

        public string Target { get; set; }

        public DateTime ReceivedDate { get; set; }
    }
}
