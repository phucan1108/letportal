using System;
using System.Threading;
using System.Threading.Tasks;
using LetPortal.Core.Utils;
using LetPortal.Microservices.Client.Channels;
using LetPortal.Microservices.Client.Models;
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
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Waiting to read notification from channel");
                try
                {
                    while (await _channel.GetChannel().Reader.WaitToReadAsync(stoppingToken))
                    {
                        if (_channel.GetChannel().Reader.TryRead(out NotificationMessage log))
                        {
                            var response = await _client.SendAsync((NotificationMessageRequest)ConvertUtil.MoveValueBetweenTwoObjects(log, new NotificationMessageRequest()));
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("There are some problems when trying to stream log collector", ex.ToString());
                }
            }
        }
    }
}
