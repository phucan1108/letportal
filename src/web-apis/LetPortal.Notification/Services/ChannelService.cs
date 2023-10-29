using LetPortal.Notification.Channels;
using LetPortal.Notification.Entities;
using LetPortal.Notification.Models;
using LetPortal.Notification.Repositories.Channels;

namespace LetPortal.Notification.Services
{
    public class ChannelService : IChannelService
    {
        private readonly IChannelRepository _channelRepository;

        private readonly GlobalNotificationChannel _globalNotificationChannel;

        private readonly RolesNotificationChannel _rolesNotificationChannel;

        private readonly IndividualNotificationChannel _individualNotificationChannel;

        public ChannelService(
            IChannelRepository channelRepository,
            GlobalNotificationChannel globalNotificationChannel,
            RolesNotificationChannel rolesNotificationChannel,
            IndividualNotificationChannel individualNotificationChannel)
        {
            _channelRepository = channelRepository;
            _globalNotificationChannel = globalNotificationChannel;
            _rolesNotificationChannel = rolesNotificationChannel;
            _individualNotificationChannel = individualNotificationChannel;
        }

        public async Task<IEnumerable<Channel>> GetAllAvailableChannels()
        {
            return await _channelRepository.GetAllAsync(a => a.Active);
        }

        public async Task<Channel> GetByCode(string code)
        {
            return await _channelRepository.FindAsync(a => a.Code == code);
        }

        public async Task Push(IncomingMessage incomingMessage)
        {
            if (incomingMessage is GlobalIncomingMessage)
            {
                await _globalNotificationChannel.GetChannel().Writer.WriteAsync((GlobalIncomingMessage)incomingMessage);
            }
            else if (incomingMessage is RoleIncomingMessage)
            {
                await _rolesNotificationChannel.GetChannel().Writer.WriteAsync((RoleIncomingMessage)incomingMessage);
            }
            else if (incomingMessage is IndividualIncomingMessage)
            {
                await _individualNotificationChannel.GetChannel().Writer.WriteAsync((IndividualIncomingMessage)incomingMessage);
            }
        }
    }
}
