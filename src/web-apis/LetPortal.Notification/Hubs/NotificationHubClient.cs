using LetPortal.Core.Logger;
using LetPortal.Notification.Channels;
using LetPortal.Notification.Models.RealTimes;
using LetPortal.Notification.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace LetPortal.Notification.Hubs
{
    [Authorize]
    public class NotificationHubClient : Hub<INotificationHubClient>
    {
        private readonly NotificationRealTimeContext _realtimeContext;

        private readonly ISubcriberService _subcriberService;

        private readonly IServiceLogger<NotificationHubClient> _logger;

        public NotificationHubClient(
            NotificationRealTimeContext realtimeContext,
            IServiceLogger<NotificationHubClient> logger,
            ISubcriberService subcriberService)
        {
            _realtimeContext = realtimeContext;
            _logger = logger;
            _subcriberService = subcriberService;
        }

        public Task Online(OnlineSubcriber onlineSubcriber)
        {
            _realtimeContext.AddSubcriber(onlineSubcriber);
            return Task.CompletedTask;
        }

        public async Task ClickedOnNotificationbox(string subcriberId)
        {
            var clickedTs = DateTime.UtcNow.Ticks;
            await _subcriberService.ClickOnNotificationBox(subcriberId, DateTime.UtcNow.Ticks);
            _realtimeContext.ClickedOnBox(subcriberId, clickedTs);
        }

        public async Task MarkRead(OnlineSubcriber onlineSubcriber, string notificationMessageId)
        {
            // mark in context
            _realtimeContext.MarkRead(onlineSubcriber.SubcriberId, notificationMessageId);

            // Update in database
            await _subcriberService.MarkRead(onlineSubcriber.SubcriberId, notificationMessageId);

            // Broadcast change to all connections
            await Clients.User(onlineSubcriber.UserName).MarkRead(notificationMessageId);
        }

        public Task Offline(OnlineSubcriber onlineSubcriber)
        {
            return Task.CompletedTask;
        } 

        public Task PermanentOffline(OnlineSubcriber onlineSubcriber)
        {
            _realtimeContext.RemoveSubcriber(onlineSubcriber);

            return Task.CompletedTask;

        }
    }
}
