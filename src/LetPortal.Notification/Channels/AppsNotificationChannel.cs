using System.Threading.Channels;
using LetPortal.Notification.Entities;
using LetPortal.Notification.Models;

namespace LetPortal.Notification.Channels
{
    public class AppsNotificationChannel : INotificationChannel<AppIncomingMessage>
    {
        public NotificationScope Scope => NotificationScope.Apps;

        private readonly Lazy<Channel<AppIncomingMessage>> _channelLazy
           = new(() => System.Threading.Channels.Channel.CreateBounded<AppIncomingMessage>(10000));

        public Channel<AppIncomingMessage> GetChannel()
        {
            return _channelLazy.Value;
        }
    }
}
