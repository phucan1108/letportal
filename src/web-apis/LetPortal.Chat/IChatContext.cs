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

        OnlineUser GetOnlineUser(string userName);

        Task TakeOnlineAsync(OnlineUser user);

        Task TakeOfflineAsync(OnlineUser user);

        void LoadDoubleRoom(ChatRoomModel chatRoom);

        ChatRoomModel GetDoubleRoom(OnlineUser invitor, OnlineUser invitee);

        ChatRoomModel CreateDoubleRoom(OnlineUser invitor, OnlineUser invitee);

        void AddChatRoomSession(ChatSessionModel chatSession);

        void SendMessage(string chatSessionId, MessageModel message);
    }
}
