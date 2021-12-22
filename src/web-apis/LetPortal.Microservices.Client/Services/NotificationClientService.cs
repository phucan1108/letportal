using System.Threading.Tasks;
using LetPortal.Microservices.Client.Channels;
using LetPortal.Microservices.Client.Models;

namespace LetPortal.Microservices.Client.Services
{
    public class NotificationClientService : INotificationClientService
    {
        private readonly INotificationChannel _channel;

        public NotificationClientService(
             INotificationChannel channel)
        {
            _channel = channel;
        }

        public async Task Send(NotificationMessage notificationMessage)
        {
            await _channel.GetChannel().Writer.WriteAsync(notificationMessage);
        }
    }
}
