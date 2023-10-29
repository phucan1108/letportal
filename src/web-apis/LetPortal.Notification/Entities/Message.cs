using LetPortal.Core.Notifications;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;

namespace LetPortal.Notification.Entities
{
    [EntityCollection(Name = "notification_messages")]
    public class Message : Entity
    {
        public string MessageGroupId { get; set; }

        public NotificationType Type { get; set; }

        public string EncodedMessage { get; set; }

        public DateTime ReceivedDate { get; set; }

        public DateTime ReadDate { get; set; }
    }
}
