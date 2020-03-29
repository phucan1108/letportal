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
        IList<OnlineUser> GetOnlineUsers();

        Task TakeOnlineAsync(OnlineUser user);

        Task TakeOfflineAsync(OnlineUser user);

        void LoadDoubleRoom(ChatRoomModel chatRoom);

        ChatRoomModel CreateDoubleRoom(OnlineUser invitor, OnlineUser invitee);

        ChatSessionModel GetLastChatRoomSession(string chatRoomId);

        ChatSessionModel InitChatRoomSession(string chatRoomId, string previousSessionId = null);

        void SendMessage(string chatSessionId, MessageModel message);
    }
}
