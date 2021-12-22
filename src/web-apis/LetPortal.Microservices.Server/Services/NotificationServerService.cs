using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using LetPortal.Core.Logger;
using LetPortal.Core.Utils;
using LetPortal.Microservices.Notifications;
using LetPortal.Microservices.Server.Repositories.Abstractions;

namespace LetPortal.Microservices.Server.Services
{
    public class NotificationServerService : NotificationService.NotificationServiceBase
    {
        private readonly IServiceLogger<NotificationServerService> _logger;

        private readonly INotificationMessageQueueRepository _notificationMessageQueueRepository;

        public NotificationServerService(
            IServiceLogger<NotificationServerService> logger,
            INotificationMessageQueueRepository notificationMessageQueueRepository
            )
        {
            _logger = logger;
            _notificationMessageQueueRepository = notificationMessageQueueRepository;
        }

        public override async Task<NotificationMessageResponse> Send(NotificationMessageRequest request, ServerCallContext context)
        {
            await _notificationMessageQueueRepository.AddAsync(new Entities.NotificationMessageQueue
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
    }
}
