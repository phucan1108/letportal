using System.Threading.Channels;
using LetPortal.Notification.Entities;
using LetPortal.Notification.Models;

namespace LetPortal.Notification.Channels
{
    public class RolesNotificationChannel : INotificationChannel<RoleIncomingMessage>
    {
        public NotificationScope Scope => NotificationScope.Roles;

        private readonly Lazy<Channel<RoleIncomingMessage>> _channelLazy
           = new(() => System.Threading.Channels.Channel.CreateBounded<RoleIncomingMessage>(10000));

        public Channel<RoleIncomingMessage> GetChannel()
        {
            return _channelLazy.Value;
        }
    }
}
