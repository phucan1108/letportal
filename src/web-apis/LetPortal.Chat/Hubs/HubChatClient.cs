using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Chat.Entities;
using LetPortal.Chat.Models;
using LetPortal.Chat.Repositories.ChatRooms;
using LetPortal.Chat.Repositories.ChatSessions;
using LetPortal.Chat.Repositories.ChatUsers;
using LetPortal.Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace LetPortal.Chat.Hubs
{
    [Authorize]
    public class HubChatClient : Hub<IHubChatClient>
    {
        private readonly IChatContext _chatContext;

        private readonly IChatRoomRepository _chatRoomRepository;

        private readonly IChatSessionRepository _chatSessionRepository;

        private readonly IChatUserRepository _chatUserRepository;

        public HubChatClient(
            IChatContext chatContext,
            IChatRoomRepository chatRoomRepository,
            IChatSessionRepository chatSessionRepository,
            IChatUserRepository chatUserRepository)
        {
            _chatContext = chatContext;
            _chatRoomRepository = chatRoomRepository;
            _chatSessionRepository = chatSessionRepository;
            _chatUserRepository = chatUserRepository;
        }
        
        public async Task OpenDoubleChatRoom(Models.OnlineUser invitee)
        {
            // Check this room is existed or not
            var invitor = _chatContext.GetOnlineUser(Context.UserIdentifier);
            var founDoubleRoom = _chatContext.GetDoubleRoom(invitor, invitee);
            string chatRoomId = string.Empty;
            bool isExistedOnDb = false;
            var chatSessionModel = new ChatSessionModel
            {
                SessionId = DataUtil.GenerateUniqueId(),
                Messages = new Queue<MessageModel>()                
            };
            ChatSessionModel previousSessionModel = null;
            if(founDoubleRoom == null)
            {
                // Try to fetch from database
                var foundRoomInDb = (await _chatRoomRepository
                    .GetAllAsync(a => a.Type == RoomType.Double
                        && a.Participants.Any(b => b.Username == Context.UserIdentifier)
                        && a.Participants.Any(c => c.Username == invitee.UserName))).FirstOrDefault();
                if (foundRoomInDb != null)
                {
                    _chatContext.LoadDoubleRoom(new Models.ChatRoomModel
                    {
                        ChatRoomId = foundRoomInDb.Id,
                        Participants = new System.Collections.Generic.List<Models.OnlineUser>
                        {
                            invitor,
                            invitee
                        },
                        RoomName = invitee.FullName,
                        Type = RoomType.Double
                    });

                    isExistedOnDb = true;
                    chatRoomId = foundRoomInDb.Id;
                }
                else
                {
                    // Create new chatroom
                    var chatRoom = new ChatRoom
                    {
                        Id = DataUtil.GenerateUniqueId(),
                        CreatedDate = DateTime.UtcNow,
                        Participants = new List<Participant>
                        {
                            new Participant
                            {
                                Id = DataUtil.GenerateUniqueId(),
                                JoinedDate = DateTime.UtcNow,
                                Username = invitor.UserName
                            },
                            new Participant
                            {
                                Id = DataUtil.GenerateUniqueId(),
                                JoinedDate = DateTime.UtcNow,
                                Username = invitee.UserName
                            }
                        },
                        RoomName = invitee.UserName,
                        Sessions = new List<ChatSession>(),
                        Type = RoomType.Double
                    };

                    await _chatRoomRepository.AddAsync(chatRoom);
                    _chatContext.LoadDoubleRoom(new Models.ChatRoomModel
                    {
                        ChatRoomId = chatRoom.Id,
                        Participants = new System.Collections.Generic.List<Models.OnlineUser>
                        {
                            invitor,
                            invitee
                        },
                        RoomName = invitee.FullName,
                        Type = RoomType.Double
                    });
                    chatRoomId = chatRoom.Id;
                }
            }
            else
            {
                chatRoomId = founDoubleRoom.ChatRoomId;
            }

            if (isExistedOnDb)
            {
                // Try to fetch last chat session
                var foundLastSession = await _chatSessionRepository.GetLastChatSession(chatRoomId);
                if(foundLastSession != null)
                {
                    previousSessionModel = new ChatSessionModel
                    {
                        ChatRoomId = chatRoomId,
                        Messages = new Queue<MessageModel>(),
                        PreviousSessionId = foundLastSession.PreviousSessionId,
                        NextSessionId = chatSessionModel.SessionId
                    };

                    if(foundLastSession.Conversations != null)
                    {
                        foreach (var message in foundLastSession.Conversations.OrderBy(a => a.Timestamp))
                        {
                            previousSessionModel.Messages.Enqueue(new MessageModel
                            {
                                Message = message.Message,
                                FormattedMessage = message.MessageTransform,
                                TimeStamp = message.Timestamp,
                                UserName = message.Username,
                                FileUrls = message.FileUrl.Split("|")
                            });
                        }
                    }
                    
                    chatSessionModel.PreviousSessionId = foundLastSession.PreviousSessionId;
                    _chatContext.AddChatRoomSession(previousSessionModel);
                }
            }

            chatSessionModel.ChatRoomId = chatRoomId;
            _chatContext.AddChatRoomSession(chatSessionModel);
            // Allow target user to prepare a chatroom
            await Clients.User(invitee.UserName).ReadyDoubleChatRoom(chatSessionModel, invitor, previousSessionModel);

            await Clients.Caller.ReadyDoubleChatRoom(chatSessionModel, invitee, previousSessionModel);
        } 

        public async Task SendMessage(string chatSessionId, string receiver, MessageModel message)
        {
            // Centralized timestamp
            message.TimeStamp = DateTime.UtcNow.Ticks;
            _chatContext.SendMessage(chatSessionId, message);

            await Clients.User(receiver).ReceivedMessage(chatSessionId, message);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // When user closes a browser tab or call stop() connection
            // we need to ensure to notify all users about offline state

            // This method is a same logic of Offline
            var isOffline = await _chatContext.TakeOfflineAsync(new OnlineUser
            {
                UserName = Context.UserIdentifier
            });

            if (isOffline)
            {
                await Clients.Others.Offline(Context.UserIdentifier);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
