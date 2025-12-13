using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;

namespace LetPortal.Notification.Entities
{
    [EntityCollection(Name = "nofitication_message_groups")]
    public class MessageGroup : Entity
    {
        public string Name { get; set; }

        public string SubcriberId { get; set; }

        public string ChannelCode { get; set; }

        public string Icon { get; set; }

        public bool Mute { get; set; }

        public long LastVisitedTs { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
