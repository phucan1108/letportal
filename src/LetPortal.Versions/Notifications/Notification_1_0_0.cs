using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Notifications;
using LetPortal.Core.Utils;
using LetPortal.Core.Versions;
using LetPortal.Notification.Entities;
using LetPortal.Notification.Repositories.Channels;
using LetPortal.Notification.Repositories.MessageGroups;
using LetPortal.Notification.Repositories.Subcribers;
using LetPortal.Portal;

namespace LetPortal.Versions.Notifications
{
    public class Notification_1_0_0 : IPortalVersion
    {
        public string VersionNumber => "1.0.0";

        private readonly IChannelRepository _channelRepository;

        private readonly IMessageGroupRepository _messageGroupRepository;

        private readonly ISubscriberRepository _subscriberRepository;

        public Notification_1_0_0(
            IChannelRepository channelRepository,
            IMessageGroupRepository messageGroupRepository,
            ISubscriberRepository subscriberRepository)
        {
            _channelRepository = channelRepository;
            _messageGroupRepository = messageGroupRepository;
            _subscriberRepository = subscriberRepository;
        }

        public Task Downgrade(IVersionContext versionContext)
        {
            return Task.CompletedTask;
        }

        public async Task Upgrade(IVersionContext versionContext)
        {
            await _channelRepository.AddAsync(new Notification.Entities.Channel
            {
                Id = DataUtil.GenerateUniqueId(),
                Code = "General",
                Active = true,
                AllowedTypes = new NotificationType[] { NotificationType.Info, NotificationType.Critical, NotificationType.Warning },
                Scope = NotificationScope.Global,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                Description = "General notification",
                Icon = "system_security_update_warning",
                Name = "General"
            });
            var adminSubcriber = new Subcriber
            {
                Id = DataUtil.GenerateUniqueId(),
                Active = true,
                LastClickedBoxTs = 0,
                Roles = new List<string> { "SuperAdmin" },
                UserId = "5ce287ee569d6f23e8504cef",
                UserName = "admin"
            };
            await _subscriberRepository.AddAsync(adminSubcriber);
            await _messageGroupRepository.AddAsync(new MessageGroup
            {
                Id = DataUtil.GenerateUniqueId(),
                ChannelCode = "General",
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                Icon = "system_security_update_warning",
                LastVisitedTs = 0,
                Mute = false,
                Name = "General",
                SubcriberId = adminSubcriber.Id
            });
        }
    }
}
