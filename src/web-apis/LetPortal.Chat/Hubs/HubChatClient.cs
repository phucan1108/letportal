﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Chat.Configurations;
using LetPortal.Chat.Entities;
using LetPortal.Chat.Models;
using LetPortal.Chat.Repositories.ChatRooms;
using LetPortal.Chat.Repositories.ChatSessions;
using LetPortal.Chat.Repositories.ChatUsers;
using LetPortal.Core.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace LetPortal.Chat.Hubs
{
    [Authorize]
    public class HubChatClient : Hub<IHubChatClient>
    {
        private readonly IChatContext _chatContext;

        private readonly IChatRoomRepository _chatRoomRepository;

        private readonly IChatSessionRepository _chatSessionRepository;

        private readonly IChatUserRepository _chatUserRepository;

        private readonly IOptionsMonitor<ChatOptions> _chatOptions;

        public HubChatClient(
            IChatContext chatContext,
            IChatRoomRepository chatRoomRepository,
            IChatSessionRepository chatSessionRepository,
            IChatUserRepository chatUserRepository,
            IOptionsMonitor<ChatOptions> chatOptions)
        {
            _chatContext = chatContext;
            _chatRoomRepository = chatRoomRepository;
            _chatSessionRepository = chatSessionRepository;
            _chatUserRepository = chatUserRepository;
            _chatOptions = chatOptions;
        }
        
        public async Task OpenDoubleChatRoom(Models.OnlineUser invitee)
        {
            // Check this room is existed or not
            var invitor = _chatContext.GetOnlineUser(Context.UserIdentifier);
            var founDoubleRoom = _chatContext.GetDoubleRoom(invitor, invitee);
            string chatRoomId = string.Empty;
            string chatSessionId = string.Empty;
            bool isExistedOnDb = false;
            bool createNewSession = false;
            ChatSessionModel chatSessionModel = null;
            ChatSessionModel previousSessionModel = null;
            ChatRoomModel chatRoomModel = null;
            
            if(founDoubleRoom == null)
            {
                // Try to fetch from database
                var foundRoomInDb = (await _chatRoomRepository
                    .GetAllAsync(a => a.Type == RoomType.Double
                        && a.Participants.Any(b => b.Username == Context.UserIdentifier)
                        && a.Participants.Any(c => c.Username == invitee.UserName))).FirstOrDefault();
                if (foundRoomInDb != null)
                {
                    chatRoomModel = new Models.ChatRoomModel
                    {
                        ChatRoomId = foundRoomInDb.Id,
                        Participants = new System.Collections.Generic.List<Models.OnlineUser>
                        {
                            invitor,
                            invitee
                        },
                        RoomName = invitee.FullName,
                        Type = RoomType.Double
                    };
                    _chatContext.LoadDoubleRoom(chatRoomModel);

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
                    chatRoomModel = new Models.ChatRoomModel
                    {
                        ChatRoomId = chatRoom.Id,
                        Participants = new System.Collections.Generic.List<Models.OnlineUser>
                        {
                            invitor,
                            invitee
                        },
                        RoomName = invitee.FullName,
                        Type = RoomType.Double
                    };
                    _chatContext.LoadDoubleRoom(chatRoomModel);
                    chatRoomId = chatRoom.Id;
                }
            }
            else
            {
                chatRoomId = founDoubleRoom.ChatRoomId;
                chatRoomModel = founDoubleRoom;
                chatSessionModel = _chatContext.GetCurrentChatSession(chatRoomId);
                if(chatSessionModel == null)
                {
                    createNewSession = true;
                }
                else if(!string.IsNullOrEmpty(chatSessionModel.PreviousSessionId))
                {
                    previousSessionModel = _chatContext.GetChatSession(chatSessionModel.PreviousSessionId);
                }
            }

            if (isExistedOnDb)
            {
                // Try to fetch last chat session
                var foundLastSession = await _chatSessionRepository.GetLastChatSession(chatRoomId);
                if(foundLastSession != null)
                {
                    // In mind, we only create new chat session when it reaches Threshold
                    // Or it belongs to previous day
                    if (foundLastSession.Conversations.Count >= _chatOptions.CurrentValue.ThresholdNumberOfMessages
                        || foundLastSession.LeaveDate.Date < DateTime.UtcNow.Date)
                    {
                        createNewSession = true;
                        // Load previous session
                        previousSessionModel = new ChatSessionModel
                        {
                            ChatRoomId = chatRoomId,
                            Messages = new Queue<MessageModel>(),
                            CreatedDate = foundLastSession.CreatedDate,
                            PreviousSessionId = foundLastSession.PreviousSessionId
                        };

                        if (foundLastSession.Conversations != null)
                        {
                            foreach (var message in foundLastSession.Conversations.OrderBy(a => a.Timestamp))
                            {
                                previousSessionModel.Messages.Enqueue(new MessageModel
                                {
                                    Message = message.Message,
                                    FormattedMessage = message.MessageTransform,
                                    TimeStamp = message.Timestamp,
                                    UserName = message.Username,
                                    CreatedDate = message.CreatedDate,
                                    FileUrls = message.FileUrl.Split("|")
                                });
                            }
                        }

                        _chatContext.AddChatRoomSession(previousSessionModel);
                    }
                    else
                    {
                        chatSessionModel = new ChatSessionModel
                        {
                            ChatRoomId = chatRoomId,
                            Messages = new Queue<MessageModel>(),
                            PreviousSessionId = foundLastSession.PreviousSessionId
                        };

                        if (foundLastSession.Conversations != null)
                        {
                            foreach (var message in foundLastSession.Conversations.OrderBy(a => a.Timestamp))
                            {
                                chatSessionModel.Messages.Enqueue(new MessageModel
                                {
                                    Message = message.Message,
                                    FormattedMessage = message.MessageTransform,
                                    TimeStamp = message.Timestamp,
                                    UserName = message.Username,
                                    CreatedDate = message.CreatedDate,
                                    FileUrls = message.FileUrl.Split("|")
                                });
                            }
                        }

                        _chatContext.AddChatRoomSession(chatSessionModel);
                    }
                }
                else
                {
                    createNewSession = true;
                }
            }

            if (createNewSession)
            {
                chatSessionModel = new ChatSessionModel
                {
                    SessionId = DataUtil.GenerateUniqueId(),
                    ChatRoomId = chatRoomId,
                    Messages = new Queue<MessageModel>(),
                    CreatedDate = DateTime.UtcNow,
                    PreviousSessionId = previousSessionModel?.SessionId
                };

                if(previousSessionModel != null)
                {
                    previousSessionModel.NextSessionId = chatSessionModel.SessionId;
                }                 
                                
                _chatContext.AddChatRoomSession(chatSessionModel);
            }
            
            // Allow target user to prepare a chatroom
            await Clients
                .User(invitee.UserName)
                .ReadyDoubleChatRoom(chatSessionModel, invitor, previousSessionModel);

            await Clients.Caller.LoadDoubleChatRoom(
                chatRoomModel,
                chatSessionModel, 
                invitee, 
                previousSessionModel);
        }

        public async Task SendChatMessage(SendMessageModel model)
        {
            // Centralized timestamp
            model.Message.TimeStamp = DateTime.UtcNow.Ticks;
            model.Message.CreatedDate = DateTime.UtcNow;
            _chatContext.SendMessage(model.ChatSessionId, model.Message);

            await Clients.User(model.Receiver).ReceivedMessage(model.ChatSessionId, model.Message);
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
