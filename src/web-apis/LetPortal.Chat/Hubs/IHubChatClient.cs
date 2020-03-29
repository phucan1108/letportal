using System.Threading.Tasks;
using LetPortal.Chat.Models;

namespace LetPortal.Chat.Hubs
{
    public interface IHubChatClient
    {
        Task Online(OnlineUser onlineUser);

        Task Offline(string userName);
    }
}
