using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetPortal.Notification.Models.RealTimes
{
    public class OnlineMessageGroup
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string SubcriberId { get; set; }

        public string ChannelCode { get; set; }

        public string Icon { get; set; }

        public bool Mute { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        public long LastVisitedTs { get; set; }

        public int NumberOfUnreadMessages { get; set; }

        public OnlineNotificationMessage LastMessage { get; set; }

        public IList<OnlineNotificationMessage> Messages { get; set; }
    }
}
