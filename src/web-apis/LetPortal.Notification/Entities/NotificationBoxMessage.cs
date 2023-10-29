using LetPortal.Core.Notifications;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;

namespace LetPortal.Notification.Entities
{

    [EntityCollection(Name = "notification_box_messages")]
    public class NotificationBoxMessage : Entity
    {
        public string SubcriberId { get; set; }

        public string MessageGroupId { get; set; }

        public string MessageId { get; set; }

        public NotificationType Type { get; set; }

        public string ShortMessage { get; set; }

        public bool IsDirty { get; set; }
        
        public DateTime ReceivedDate { get; set; }

        [MongoIndex(Name = "received_date_ts_desc_index", Type = MongoIndexType.Desc)]
        public long ReceivedDateTs { get; set; }

        public DateTime ClickedDate { get; set; }
    }
}
