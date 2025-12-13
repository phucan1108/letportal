using LetPortal.Core.Notifications;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;

namespace LetPortal.Notification.Entities
{
    [EntityCollection(Name = "notification_channels")]
    public class Channel : Entity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Code { get; set; }

        public NotificationScope Scope { get; set; }

        public IList<NotificationType> AllowedTypes { get; set; }

        public string Icon { get; set; }

        /// <summary>
        /// Use when Scope = Roles
        /// </summary>
        public IList<string> Roles { get; set; }

        /// <summary>
        /// Use when Scope = Apps
        /// </summary>
        public IList<string> Apps { get; set; }

        /// <summary>
        /// Use when Scope = Individual
        /// </summary>
        public string UserId { get; set; }

        public bool Active { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }
    }

    public enum NotificationScope
    {
        Global,
        Roles,
        Apps,
        Individual
    }
}
