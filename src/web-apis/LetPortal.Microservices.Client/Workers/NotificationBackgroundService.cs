using System;
using System.Threading;
using System.Threading.Tasks;
using LetPortal.Core.Utils;
using LetPortal.Microservices.Client.Channels;
using LetPortal.Notification;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LetPortal.Microservices.Client.Workers
{
    public class NotificationBackgroundService : BackgroundService
    {
        private readonly IOptionsMonitor<ServiceOptions> _options;

        private readonly INotificationChannel _channel;

        private readonly ILogger<NotificationBackgroundService> _logger;

        private readonly NotificationService.NotificationServiceClient _client;

        public NotificationBackgroundService(
            IOptionsMonitor<ServiceOptions> options,
            ILogger<NotificationBackgroundService> logger,
            INotificationChannel channel,
            NotificationService.NotificationServiceClient client)
        {
            _options = options;
            _channel = channel;
            _logger = logger;
            _client = client;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting worker for pushing notification to Saturn at endpoint " + _options.CurrentValue.SaturnEndpoint);
            try
            {
                await foreach (var notificationMessage in _channel.GetChannel().Reader.ReadAllAsync(stoppingToken))
                {
                    var response = await _client.SendAsync((NotificationMessageRequest)ConvertUtil.MoveValueBetweenTwoObjects(notificationMessage, new NotificationMessageRequest()));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There are some problems when trying to send notification");
            }
        }
    }
}
