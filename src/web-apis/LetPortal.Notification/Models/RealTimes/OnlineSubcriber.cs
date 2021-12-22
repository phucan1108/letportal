using LetPortal.Notification.Entities;

namespace LetPortal.Notification.Models.RealTimes
{
    public class OnlineSubcriber
    {
        public string UserId { get; set; }

        public string SubcriberId { get; set; }

        public string UserName { get; set; }

        public long LastClickedTs { get; set; }

        public int LastUnreadMessages { get; set; }

        public IList<string> Roles { get; set; }

        public IList<OnlineMessageGroup> Groups { get; set; } = new List<OnlineMessageGroup>();
    }
}
