using System;
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
        private readonly List<OnlineUser> onlineUsers;

        private readonly List<ChatRoomModel> chatRooms;

        private readonly List<ChatSessionModel> chatSessions;

        private readonly IOptionsMonitor<ChatOptions> _options;

        public ChatContext(IOptionsMonitor<ChatOptions> options)
        {
            onlineUsers = new List<OnlineUser>();
            chatRooms = new List<ChatRoomModel>();
            chatSessions = new List<ChatSessionModel>();
            _options = options;
        }

        public IEnumerable<OnlineUser> GetOnlineUsers()
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

            if (foundSession.IsInDb && !foundSession.IsDirty)
            {
                foundSession.IsDirty = true;                
            }
            foundSession.LastMessageDate = DateTime.UtcNow;
            foundSession.Messages.Enqueue(message);
        }

        public Task<bool> TakeOfflineAsync(OnlineUser user)
        {
            if (onlineUsers.Any(a => a.UserName == user.UserName))
            {
                var found = onlineUsers.Find(a => a.UserName == user.UserName);
                found.NumberOfDevices -= 1;
                if (found.NumberOfDevices == 0)
                {   
                    onlineUsers.Remove(found);
                    return Task.FromResult(true);
                }
            }
            return Task.FromResult(false);
        }

        public Task TakeOnlineAsync(OnlineUser user)
        {
            if(onlineUsers.Any(a => a.UserName == user.UserName))
            {
                var found = onlineUsers.Find(a => a.UserName == user.UserName);
                // Because SignalR allows us to have multiple devices per one user (indicated by username)
                // So we need to increase number of devices to decide when user is offline
                found.NumberOfDevices += 1;
            }
            else
            {
                user.NumberOfDevices = 1;
                onlineUsers.Add(user);
            }                                    

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
            var numberOfSessions = chatSessions.Count(a => a.ChatRoomId == chatSession.ChatRoomId);
            if(numberOfSessions < _options.CurrentValue.MaximumSessionsPerChatRoom)
            {  
                chatSessions.Add(chatSession);
            }
            else
            {
                // Remove last created
                var lastSession = chatSessions.Where(a => a.ChatRoomId == chatSession.ChatRoomId).OrderBy(a => a.LastMessageDate).First();
                var lastSessionIndex = chatSessions.IndexOf(lastSession);
                chatSessions.RemoveAt(lastSessionIndex);
                chatSessions.Add(chatSession);
            }
        }

        public ChatSessionModel GetCurrentChatSession(string chatRoomId)
        {
            return chatSessions
                    .Where(b => b.ChatRoomId == chatRoomId)
                    .OrderByDescending(a => a.CreatedDate)
                    .FirstOrDefault();
        }

        public ChatSessionModel GetChatSession(string chatSessionId)
        {
            return chatSessions.FirstOrDefault(a => a.SessionId == chatSessionId);
        }

        public bool WantToAddNewSession(string chatSessionId)
        {
            return chatSessions.Any(a => 
                    a.SessionId == chatSessionId 
                    && (
                        (a.Messages != null && a.Messages.Count >= _options.CurrentValue.ThresholdNumberOfMessages)
                        || a.CreatedDate.Date < DateTime.UtcNow.Date));
        }

        public IEnumerable<ChatSessionModel> GetAllActiveSessions(string userName)
        {
            var allRooms = chatRooms.Where(a => a.Participants.Any(b => b.UserName == userName)).Select(c => c.ChatRoomId);
            return chatSessions.Where(a => 
                allRooms.Contains(a.ChatRoomId) 
                && (!a.IsInDb || (a.IsInDb && a.IsDirty))
                && a.Messages.Count > 0);

        }

        public void CloseAllUnlistenRooms(string relatedUser)
        {
            var allUnlistenRooms = chatRooms.Where(a => a.NoListener(onlineUsers, relatedUser)).Select(b => b.ChatRoomId).ToList();
            if(allUnlistenRooms != null)
            {
                foreach(var roomId in allUnlistenRooms)
                {
                    var found = chatRooms.Find(a => a.ChatRoomId == roomId);
                    chatRooms.Remove(found);
                }
            }               
        }
    }
}
