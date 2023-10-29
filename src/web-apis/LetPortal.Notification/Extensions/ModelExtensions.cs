using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Core.Utils;
using LetPortal.Notification.Entities;
using LetPortal.Notification.Models;
using LetPortal.Notification.Models.RealTimes;

namespace LetPortal.Notification.Extensions
{
    public static class ModelExtensions
    {
        public static Message ToMessage(this IncomingMessage incomingMessage, string messageGroupId)
        {
            return new Message
            {
                Id = DataUtil.GenerateUniqueId(),
                MessageGroupId = messageGroupId,
                EncodedMessage = incomingMessage.Content,
                ReceivedDate = incomingMessage.ReceivedDate,
                Type = incomingMessage.Type
            };
        }

        public static NotificationBoxMessage ToNotificationMessage(
            this IncomingMessage incomingMessage,
            string subcriberId,
            string messageGroupId,
            string messageId)
        {
            return new NotificationBoxMessage
            {
                Id = DataUtil.GenerateUniqueId(),
                IsDirty = false,
                ReceivedDate = incomingMessage.ReceivedDate,
                ReceivedDateTs = incomingMessage.ReceivedDate.Ticks,
                MessageGroupId = messageGroupId,
                MessageId = messageId,
                ShortMessage = incomingMessage.Content.Length <= Constants.MAXIMUM_CONTENT_SHORT_MESSAGE ? incomingMessage.Content : incomingMessage.Content.Substring(0, Constants.MAXIMUM_CONTENT_SHORT_MESSAGE - 1),
                SubcriberId = subcriberId,
                Type = incomingMessage.Type
            };
        }

        public static OnlineNotificationMessage ToOnline(
            this NotificationBoxMessage notificationBoxMessage)
        {

            return new OnlineNotificationMessage
            {
                MessageGroupId = notificationBoxMessage.MessageGroupId,
                ClickedDate = notificationBoxMessage.ClickedDate,
                IsDirty = notificationBoxMessage.IsDirty,
                MessageId = notificationBoxMessage.MessageId,
                NotificationBoxId = notificationBoxMessage.Id,
                ReceivedDate = notificationBoxMessage.ReceivedDate,
                ReceivedDateTs = notificationBoxMessage.ReceivedDateTs,
                ShortMessage = notificationBoxMessage.ShortMessage,
                SubcriberId = notificationBoxMessage.SubcriberId,
                Type = notificationBoxMessage.Type
            };
        }

        public static OnlineMessageGroup ToOnline(
            this MessageGroup messageGroup)
        {
            return new OnlineMessageGroup
            {
                Id = messageGroup.Id,
                ChannelCode = messageGroup.ChannelCode,
                CreatedDate = messageGroup.CreatedDate,
                Icon = messageGroup.Icon,
                LastVisitedTs = messageGroup.LastVisitedTs,
                ModifiedDate = messageGroup.ModifiedDate,
                Mute = messageGroup.Mute,
                Name = messageGroup.Name,
                SubcriberId = messageGroup.SubcriberId
            };
        }
    }
}
