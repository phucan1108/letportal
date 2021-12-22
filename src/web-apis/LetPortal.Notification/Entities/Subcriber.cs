using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;

namespace LetPortal.Notification.Entities
{
    /// <summary>
    /// 
    /// </summary>
    [EntityCollection(Name = "notification_subcribers")]
    public class Subcriber : Entity
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public IList<string> Roles { get; set; }

        public bool Active { get; set; }

        public long LastClickedBoxTs { get; set; }
    }
}
