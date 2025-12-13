using LetPortal.Core.Notifications;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;

namespace LetPortal.Notification.Entities
{
    [EntityCollection(Name = "incoming_notification_messages")]
    public class IncomingNotificationMessage : Entity
    {
        public string ServiceId { get; set; }

        public string ServiceName { get; set; }

        public NotificationType NotificationType { get; set; }

        public string Sender { get; set; }

        public string ChannelCode { get; set; }

        public string Message { get; set; }

        public string IndividualUsername { get; set; }

        public DateTime SentDate { get; set; }
    }
}
