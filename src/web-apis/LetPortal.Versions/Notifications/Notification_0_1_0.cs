using System;
using System.Threading.Tasks;
using LetPortal.Core.Notifications;
using LetPortal.Core.Utils;
using LetPortal.Core.Versions;
using LetPortal.Notification.Entities;
using LetPortal.Notification.Repositories.Channels;
using LetPortal.Portal;

namespace LetPortal.Versions.Notifications
{
    public class Notification_0_1_0 : IPortalVersion
    {
        public string VersionNumber => "0.9.0";

        private readonly IChannelRepository _channelRepository;

        public Notification_0_1_0(IChannelRepository channelRepository)
        {
            _channelRepository = channelRepository;
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
        }
    }
}
