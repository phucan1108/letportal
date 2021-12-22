using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Core.Logger;
using LetPortal.Core.Notifications;
using LetPortal.Notification.Channels;
using LetPortal.Notification.Models;
using LetPortal.Notification.Repositories.NotificationMessageQueues;
using LetPortal.Notification.Services;
using Microsoft.Extensions.Hosting;

namespace LetPortal.Notification.Workers
{
    public class MessageQueueBackgroundService : BackgroundService
    {
        private readonly INotificationMessageQueueRepository _notificationMessageQueueRepository;

        private readonly IChannelService _channelService;

        private readonly IServiceLogger<MessageQueueBackgroundService> _logger;

        public MessageQueueBackgroundService(
            INotificationMessageQueueRepository notificationMessageQueueRepository,
            IChannelService channelService,
            IServiceLogger<MessageQueueBackgroundService> logger)
        {
            _notificationMessageQueueRepository = notificationMessageQueueRepository;
            _channelService = channelService;
            _logger = logger;
        } 

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Info("Start Incoming message listener");
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.Info("Waiting to read incoming message from channel");
                try
                {
                    await _notificationMessageQueueRepository.Listen(async (message) =>
                    {
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
                catch (Exception ex)
                {
                    _logger.Error("There are some problems when trying to stream log collector {@ex}", ex);
                }
            }
        }
    }
}
