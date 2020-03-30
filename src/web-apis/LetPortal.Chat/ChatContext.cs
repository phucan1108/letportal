using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Chat.Configurations;
using LetPortal.Chat.Models;
using LetPortal.Core.Utils;
using Microsoft.Extensions.Options;

namespace LetPortal.Chat
{
    public class ChatContext : IChatContext
    {
        private List<OnlineUser> onlineUsers;

        private List<ChatRoomModel> chatRooms;

        private List<ChatSessionModel> chatSessions;

        private readonly IOptionsMonitor<ChatOptions> _options;

        public ChatContext(IOptionsMonitor<ChatOptions> options)
        {
            onlineUsers = new List<OnlineUser>();
            chatRooms = new List<ChatRoomModel>();
            chatSessions = new List<ChatSessionModel>();
            _options = options;
        }

        public ChatRoomModel CreateDoubleRoom(OnlineUser invitor, OnlineUser invitee)
        {
            var chatRoom = new ChatRoomModel
            {
                Type = Entities.RoomType.Double,
                Participants = new List<OnlineUser> { invitor, invitee },
                RoomName = invitee.FullName,
                ChatRoomId = DataUtil.GenerateUniqueId()
            };

            chatRooms.Add(chatRoom);
            return chatRoom;
        }

        public IList<OnlineUser> GetOnlineUsers()
        {
            return onlineUsers;
        }

        public void LoadDoubleRoom(ChatRoomModel chatRoom)
        {
            chatRooms.Add(chatRoom);
        }

        public void SendMessage(string chatSessionId, MessageModel message)
        {
            var foundSession = chatSessions.Find(a => a.SessionId == chatSessionId);

            foundSession.Messages.Enqueue(message);
        }

        public Task TakeOfflineAsync(OnlineUser user)
        {
            if (onlineUsers.Any(a => a.UserName == user.UserName))
            {
                var found = onlineUsers.Find(a => a.UserName == user.UserName);
                onlineUsers.Remove(found);
            }
            return Task.CompletedTask;
        }

        public Task TakeOnlineAsync(OnlineUser user)
        {
            if(onlineUsers.Any(a => a.UserName == user.UserName))
            {
                var found = onlineUsers.Find(a => a.UserName == user.UserName);
                onlineUsers.Remove(found);
            }
            onlineUsers.Add(user);

            return Task.CompletedTask;
        }

        public ChatRoomModel GetDoubleRoom(OnlineUser invitor, OnlineUser invitee)
        {
            var foundRoom = chatRooms.FirstOrDefault(a => a.Type == Entities.RoomType.Double
                && a.Participants.Any(b => b.UserName == invitor.UserName)
                && a.Participants.Any(c => c.UserName == invitee.UserName));
            return foundRoom;
        }

        public OnlineUser GetOnlineUser(string userName)
        {
            return onlineUsers.FirstOrDefault(a => a.UserName == userName);
        }

        public void AddChatRoomSession(ChatSessionModel chatSession)
        {
            chatSessions.Add(chatSession);
        }
    }
}
