using System.Threading.Channels;
using LetPortal.Notification.Entities;
using LetPortal.Notification.Models;

namespace LetPortal.Notification.Channels
{
    public class IndividualNotificationChannel : INotificationChannel<IndividualIncomingMessage>
    {
        public NotificationScope Scope => NotificationScope.Individual;

        private readonly Lazy<Channel<IndividualIncomingMessage>> _channelLazy
           = new(() => System.Threading.Channels.Channel.CreateBounded<IndividualIncomingMessage>(10000));

        public Channel<IndividualIncomingMessage> GetChannel()
        {
            return _channelLazy.Value;
        }
    }
}
