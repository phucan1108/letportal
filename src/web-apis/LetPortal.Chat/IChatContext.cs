using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Chat.Models;

namespace LetPortal.Chat
{
    /// <summary>
    /// This context will maintain everything about Online Users, Chat Rooms
    /// It will work as Singleton so that a consumed memory is huge
    /// </summary>
    public interface IChatContext
    {
        IEnumerable<OnlineUser> GetOnlineUsers();

        OnlineUser GetOnlineUser(string userName);

        Task TakeOnlineAsync(OnlineUser user);

        Task<bool> TakeOfflineAsync(OnlineUser user);

        void LoadDoubleRoom(ChatRoomModel chatRoom);

        ChatRoomModel GetDoubleRoom(OnlineUser invitor, OnlineUser invitee);

        void AddChatRoomSession(ChatSessionModel chatSession);

        ChatSessionModel GetCurrentChatSession(string chatRoomId);

        ChatSessionModel GetChatSession(string chatSessionId);

        void SendMessage(string chatSessionId, MessageModel message);

        bool WantToAddNewSession(string chatSessionId);

        IEnumerable<ChatSessionModel> GetAllActiveSessions(string userName);

        void CloseAllUnlistenRooms(string relatedUser);
    }
}
