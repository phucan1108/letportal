using System;
using System.Threading.Channels;
using LetPortal.Microservices.Client.Models;

namespace LetPortal.Microservices.Client.Channels
{
    class NotificationChannel : INotificationChannel
    {
        private readonly Lazy<Channel<NotificationMessage>> _lazy = new Lazy<Channel<NotificationMessage>>(() => Channel.CreateBounded<NotificationMessage>(100000));

        public Channel<NotificationMessage> GetChannel()
        {
            return _lazy.Value;
        }
    }
}
