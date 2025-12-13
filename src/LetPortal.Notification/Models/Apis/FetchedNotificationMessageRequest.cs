using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Core.Notifications;

namespace LetPortal.Notification.Models.Apis
{
    public class FetchedNotificationMessageRequest
    {
        public string SubcriberId { get; set; }

        public string MessageGroupId { get; set; }

        public long LastFectchedTs { get; set; }

        public NotificationType[] SelectedTypes { get; set; }
    }
}
