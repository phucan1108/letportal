using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Core.Logger;
using LetPortal.Notification.Channels;
using LetPortal.Notification.Hubs;
using LetPortal.Notification.Models;
using LetPortal.Notification.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

namespace LetPortal.Notification.Workers
{
    public class GlobalChannelBackgroundService : BackgroundService
    {
        private readonly IServiceLogger<GlobalChannelBackgroundService> _logger;

        private readonly GlobalNotificationChannel _globalChannel;

        private readonly ISubcriberService _subcriberService;

        private readonly NotificationRealTimeContext _notificationRealTimeContext;

        private readonly IHubContext<NotificationHubClient, INotificationHubClient> _hubNotificationClient;

        public GlobalChannelBackgroundService(
            IServiceLogger<GlobalChannelBackgroundService> logger,
            GlobalNotificationChannel globalChannel,
            ISubcriberService subcriberService,
            NotificationRealTimeContext notificationRealTimeContext,
            IHubContext<NotificationHubClient, INotificationHubClient> hubNotificationClient
            )
        {
            _logger = logger;
            _globalChannel = globalChannel;
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
                    while (await _globalChannel.GetChannel().Reader.WaitToReadAsync())
                    {
                        if (_globalChannel.GetChannel().Reader.TryRead(out GlobalIncomingMessage? message))
                        {
                            if(message != null)
                            {
                                // After we receive a message from API, we will send to Subcriber
                                await _subcriberService.Receive(message, async(notificationMessage) =>
                                {
                                    _notificationRealTimeContext.AddNotification(notificationMessage.SubcriberId, notificationMessage);
                                    var foundOnlineSubriber = _notificationRealTimeContext.OnlineSubcribers.FirstOrDefault(a => a.SubcriberId == notificationMessage.SubcriberId);
                                    if (foundOnlineSubriber != null)
                                    {
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
