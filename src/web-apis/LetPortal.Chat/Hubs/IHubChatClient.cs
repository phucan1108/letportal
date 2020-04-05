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
        Task ReadyDoubleChatRoom(
            ChatRoomModel chatRoom,
            ChatSessionModel currentChatSession, 
            OnlineUser invitor, 
            ChatSessionModel previousSession = null);

        Task LoadDoubleChatRoom(
            ChatRoomModel chatRoom,
            ChatSessionModel chatSession, 
            OnlineUser invitee, 
            ChatSessionModel previousSession = null);

        Task ReceivedMessage(string chatRoomId, string chatSessionId, MessageModel message);

        Task BoardcastSentMessage(string chatRoomId, string chatSessionId, string lastSentHashCode, MessageModel message);

        Task AddNewChatSession(ChatSessionModel chatSession);

        Task AddPreviousSession(ChatSessionModel chatSession);
    }
}
