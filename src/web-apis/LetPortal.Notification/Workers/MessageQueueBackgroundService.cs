using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Core.Logger;
using LetPortal.Core.Notifications;
using LetPortal.Notification.Channels;
using LetPortal.Notification.Drivers;
using LetPortal.Notification.Models;
using LetPortal.Notification.Options;
using LetPortal.Notification.Repositories.NotificationMessageQueues;
using LetPortal.Notification.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace LetPortal.Notification.Workers
{
    public class MessageQueueBackgroundService : BackgroundService
    {
        private readonly IIncomingNotificationMessageRepository _notificationMessageQueueRepository;

        private readonly IChannelService _channelService;

        private readonly IServiceLogger<MessageQueueBackgroundService> _logger;

        private readonly IEnumerable<INotificationQueueDriver> _drivers;

        private readonly IOptionsMonitor<NotificationOptions> _options;

        public MessageQueueBackgroundService(
            IIncomingNotificationMessageRepository notificationMessageQueueRepository,
            IChannelService channelService,
            IEnumerable<INotificationQueueDriver> drivers,
            IOptionsMonitor<NotificationOptions> options,
            IServiceLogger<MessageQueueBackgroundService> logger)
        {
            _notificationMessageQueueRepository = notificationMessageQueueRepository;
            _channelService = channelService;
            _logger = logger;
            _drivers = drivers;
            _options = options;
        } 

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Info("Start Incoming message listener");
            var driver = _drivers.First(a => a.Driver == _options.CurrentValue.Driver);
            await driver.StartAsync();
            await driver.SubcribeAsync(async (message) =>
            {
                _logger.Info("Received incoming message {@message}", message);
                var channel = await _channelService.GetByCode(message.ChannelCode);
                switch (channel.Scope)
                {
                    case Entities.NotificationScope.Global:
                        var globalIncomingMessage = new GlobalIncomingMessage
                        {
                            Code = message.ChannelCode,
                            Content = message.Message,
                            ReceivedDate = message.SentDate,
                            Source = message.Sender,
                            Target = message.IndividualUsername,
                            Type = message.NotificationType
                        };
                        await _channelService.Push(globalIncomingMessage);
                        break;
                    case Entities.NotificationScope.Roles:
                        var rolesIncomingMessage = new RoleIncomingMessage
                        {
                            Code = message.ChannelCode,
                            Content = message.Message,
                            ReceivedDate = message.SentDate,
                            Source = message.Sender,
                            Target = message.IndividualUsername,
                            Type = message.NotificationType
                        };
                        await _channelService.Push(rolesIncomingMessage);
                        break;
                    case Entities.NotificationScope.Individual:
                        var individualIncomingMessage = new IndividualIncomingMessage
                        {
                            Code = message.ChannelCode,
                            Content = message.Message,
                            ReceivedDate = message.SentDate,
                            Source = message.Sender,
                            Target = message.IndividualUsername,
                            Type = message.NotificationType
                        };
                        await _channelService.Push(individualIncomingMessage);
                        break;
                }
            }, stoppingToken);
        }
    }
}
