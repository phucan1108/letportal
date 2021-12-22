using LetPortal.Notification.Models.RealTimes;

namespace LetPortal.Notification
{
    public class NotificationRealTimeContext
    {
        public IList<OnlineSubcriber> OnlineSubcribers { get; set; } = new List<OnlineSubcriber>();

        public void AddNotification(string subcriberId, OnlineNotificationMessage notificationMessage)
        {
            var foundOnlineSubcriber = OnlineSubcribers.FirstOrDefault(a => a.SubcriberId == subcriberId);
            if (foundOnlineSubcriber != null)
            {
                var messageGroup = foundOnlineSubcriber.Groups.First(a => a.Id == notificationMessage.MessageGroupId);
                messageGroup.Messages.Add(notificationMessage);
                // Increase unread messages
                foundOnlineSubcriber.LastUnreadMessages++;
            }
        }

        public void AddSubcriber(OnlineSubcriber onlineSubcriber)
        {
            var foundSubcriber = OnlineSubcribers.FirstOrDefault(a => a.UserId == onlineSubcriber.UserId);
            
            if (foundSubcriber == null)
            {
                OnlineSubcribers.Add(onlineSubcriber);
            }
        }

        public void RemoveSubcriber(OnlineSubcriber onlineSubcriber)
        {
            var foundSubcriber = OnlineSubcribers.FirstOrDefault(a => a.UserId == onlineSubcriber.UserId);

            if(foundSubcriber!= null)
            {
                var index = OnlineSubcribers.IndexOf(foundSubcriber);
                OnlineSubcribers.RemoveAt(index);
            }
        }

        public void MarkRead(string subcriberId, string notificationMessageId)
        {
            var foundOnlineSubcriber = OnlineSubcribers.FirstOrDefault(a => a.SubcriberId == subcriberId);
            if (foundOnlineSubcriber != null)
            {
                //var foundMessage = foundOnlineSubcriber.Messages.FirstOrDefault(a => a.NotificationBoxId == notificationMessageId);
                //if (foundMessage != null)
                //{
                //    foundMessage.IsDirty = true;
                //}
            }
        }

        public void ClickedOnBox(string subcriberId, long lastClickedTs)
        {
            var foundOnlineSubcriber = OnlineSubcribers.FirstOrDefault(a => a.SubcriberId == subcriberId);
            if(foundOnlineSubcriber != null)
            {
                foundOnlineSubcriber.LastUnreadMessages = 0;
                foundOnlineSubcriber.LastClickedTs = lastClickedTs;
            }
        }
    }
}
