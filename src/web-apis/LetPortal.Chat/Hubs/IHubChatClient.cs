using System.Threading.Tasks;
using LetPortal.Chat.Models;

namespace LetPortal.Chat.Hubs
{
    /// <summary>
    /// This interface contains all subcribing events on Client
    /// </summary>
    public interface IHubChatClient
    {
        /// <summary>
        /// Notify new online user
        /// </summary>
        /// <param name="onlineUser">Online user</param>
        /// <returns></returns>
        Task Online(OnlineUser onlineUser);

        /// <summary>
        ///  Notify user is offline
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task Offline(string userName);

        /// <summary>
        /// Notify invitee about upcoming chat session
        /// </summary>
        /// <param name="currentChatSession"></param>
        /// <param name="invitor"></param>
        /// <param name="previousSession"></param>
        /// <returns></returns>
        Task ReadyDoubleChatRoom(ChatSessionModel currentChatSession, OnlineUser invitor, ChatSessionModel previousSession = null);

        Task ReceivedMessage(string chatSessionId, MessageModel message);
    }
}
