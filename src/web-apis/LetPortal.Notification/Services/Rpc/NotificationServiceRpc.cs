using Grpc.Core;
using LetPortal.Core.Logger;
using LetPortal.Core.Utils;
using LetPortal.Notification.Drivers;
using LetPortal.Notification.Options;
using LetPortal.Notification.Repositories.Channels;
using LetPortal.Notification.Repositories.NotificationMessageQueues;
using Microsoft.Extensions.Options;

namespace LetPortal.Notification.Services.Rpc
{
    public class NotificationServiceRpc : NotificationService.NotificationServiceBase
    {
        private readonly IServiceLogger<NotificationServiceRpc> _logger;

        private readonly IIncomingNotificationMessageRepository _notificationMessageQueueRepository;

        private readonly IChannelRepository _channelRepository;

        private readonly IEnumerable<INotificationQueueDriver> _drivers;

        private readonly IOptionsMonitor<NotificationOptions> _options;

        public NotificationServiceRpc(
            IServiceLogger<NotificationServiceRpc> logger,
            IIncomingNotificationMessageRepository notificationMessageQueueRepository,
            IChannelRepository channelRepository,
            IEnumerable<INotificationQueueDriver> drivers,
            IOptionsMonitor<NotificationOptions> options
            )
        {
            _logger = logger;
            _notificationMessageQueueRepository = notificationMessageQueueRepository;
            _channelRepository = channelRepository;
            _drivers = drivers;
            _options = options;
        }

        public override async Task<NotificationMessageResponse> Send(NotificationMessageRequest request, ServerCallContext context)
        {
            var driver = _drivers.First(a => a.Driver == _options.CurrentValue.Driver);
            await driver.StartAsync();
            await driver.PushAsync(new Entities.IncomingNotificationMessage
            {
                Id = DataUtil.GenerateUniqueId(),
                ServiceId = request.ServiceId,
                ServiceName = request.ServiceName,
                ChannelCode = request.ChannelCode,
                IndividualUsername = request.IndividualUserName,
                Message = request.Message,
                Sender = request.Sender,
                SentDate = DateTime.UtcNow
            });
            return new NotificationMessageResponse
            {
                Succeed = true
            };
        }

        public override async Task<CreateChannelResponse> Create(CreateChannelRequest request, ServerCallContext context)
        {
            var existed = await _channelRepository.IsExistAsync(a => a.Code == request.Code);
            if (!existed)
            {
                await _channelRepository.AddAsync(new Entities.Channel
                {
                    Id = DataUtil.GenerateUniqueId(),
                    Active = true,
                    Code = request.Code,
                    Icon = request.Icon,
                    Name = request.Name,
                    CreatedDate = DateTime.UtcNow,
                    Scope = Entities.NotificationScope.Global
                });

                return new CreateChannelResponse
                {
                    Succeed = true
                };
            }

            return new CreateChannelResponse
            {
                Succeed = false,
                Error = "The channel has been existed"
            };
        }
    }
}
