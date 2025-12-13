using LetPortal.Notification.Entities;
using LetPortal.Notification.Models;

namespace LetPortal.Notification.Services
{
    public interface IChannelService
    {
        Task<Channel> GetByCode(string code);

        Task<IEnumerable<Channel>> GetAllAvailableChannels();

        Task Push(IncomingMessage incomingMessage);
    }
}
