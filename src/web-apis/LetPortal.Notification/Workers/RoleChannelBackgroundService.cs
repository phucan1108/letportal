using LetPortal.Core.Logger;
using LetPortal.Notification.Channels;
using LetPortal.Notification.Hubs;
using LetPortal.Notification.Models;
using LetPortal.Notification.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

namespace LetPortal.Notification.Workers
{
    public class RoleChannelBackgroundService : BackgroundService
    {

        private readonly IServiceLogger<RoleChannelBackgroundService> _logger;

        private readonly RolesNotificationChannel _roleChannel;

        private readonly ISubcriberService _subcriberService;

        private readonly NotificationRealTimeContext _notificationRealTimeContext;

        private readonly IHubContext<NotificationHubClient, INotificationHubClient> _hubNotificationClient;

        public RoleChannelBackgroundService(
            IServiceLogger<RoleChannelBackgroundService> logger,
            RolesNotificationChannel roleChannel,
            ISubcriberService subcriberService,
            NotificationRealTimeContext notificationRealTimeContext,
            IHubContext<NotificationHubClient, INotificationHubClient> hubNotificationClient
            )
        {
            _logger = logger;
            _roleChannel = roleChannel;
            _subcriberService = subcriberService;
            _hubNotificationClient = hubNotificationClient;
            _notificationRealTimeContext = notificationRealTimeContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Info("Start Notification Channel listener");
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.Info("Waiting to read incoming message from channel");
                try
                {
                    while (await _roleChannel.GetChannel().Reader.WaitToReadAsync())
                    {
                        if (_roleChannel.GetChannel().Reader.TryRead(out RoleIncomingMessage? message))
                        {
                            if (message != null)
                            {
                                // After we receive a message from API, we will send to Subcriber
                                await _subcriberService.Receive(message, async (notificationMessage, messageGroup) =>
                                {
                                    _notificationRealTimeContext.AddNotification(notificationMessage.SubcriberId, notificationMessage);
                                    var foundOnlineSubriber = _notificationRealTimeContext.OnlineSubcribers.FirstOrDefault(a => a.SubcriberId == notificationMessage.SubcriberId);
                                    if (foundOnlineSubriber != null)
                                    {
                                        if (messageGroup != null)
                                        {
                                            // In case this is new message group, we should add the current message
                                            messageGroup.LastMessage = notificationMessage;
                                            await _hubNotificationClient.Clients.User(foundOnlineSubriber.UserId).PushNewGroup(messageGroup);
                                        }
                                        await _hubNotificationClient.Clients.User(foundOnlineSubriber.UserId).Push(notificationMessage);
                                    }
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("There are some problems when trying to stream log collector {@ex}", ex);
                }
            }
        }
    }
}
