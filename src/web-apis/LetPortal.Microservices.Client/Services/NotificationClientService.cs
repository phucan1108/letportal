using System.Threading.Tasks;
using LetPortal.Microservices.Client.Channels;
using LetPortal.Microservices.Client.Models;
using LetPortal.Notification;

namespace LetPortal.Microservices.Client.Services
{
    public class NotificationClientService : INotificationClientService
    {
        private readonly INotificationChannel _channel;

        private readonly NotificationService.NotificationServiceClient _client;

        public NotificationClientService(
             INotificationChannel channel,
             NotificationService.NotificationServiceClient client)
        {
            _channel = channel;
            _client = client;
        }

        public async Task CreateChannel(string name, string code, string icon)
        {
            await _client.CreateAsync(new CreateChannelRequest { Name = name, Code = code, Icon = icon });
        }

        public async Task Send(NotificationMessage notificationMessage)
        {
            await _channel.GetChannel().Writer.WriteAsync(notificationMessage);
        }
    }
}
