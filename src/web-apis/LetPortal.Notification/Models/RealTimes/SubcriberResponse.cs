using LetPortal.Notification.Entities;

namespace LetPortal.Notification.Models.RealTimes
{
    public class SubcriberResponse
    {
        public string SubcriberId { get; set; }

        public long LastClickedTs { get; set; }

        public int LastUnreadMessages { get; set; }

        public IList<OnlineNotificationMessage> Messages { get; set; }

        public IList<MessageGroup> Groups { get; set; }
    }
}
