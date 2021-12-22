using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using LetPortal.Notification.Entities;
using LetPortal.Notification.Models;

namespace LetPortal.Notification.Channels
{
    public class GlobalNotificationChannel : INotificationChannel<GlobalIncomingMessage>
    {
        public NotificationScope Scope => NotificationScope.Global;

        private readonly Lazy<Channel<GlobalIncomingMessage>> _channelLazy
           = new(() => System.Threading.Channels.Channel.CreateBounded<GlobalIncomingMessage>(10000));

        public Channel<GlobalIncomingMessage> GetChannel()
        {
            return _channelLazy.Value;
        }
    }
}
