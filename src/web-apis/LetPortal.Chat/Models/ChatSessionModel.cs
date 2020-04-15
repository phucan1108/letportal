using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LetPortal.Chat.Entities;
using LetPortal.Core.Utils;

namespace LetPortal.Chat.Models
{
    public class ChatSessionModel
    {
        public string SessionId { get; set; }

        public string ChatRoomId { get; set; }

        public Queue<MessageModel> Messages { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime LeaveDate { get; set; }

        public string PreviousSessionId { get; set; }

        public string NextSessionId { get; set; }

        public bool IsInDb { get; set; }

        public bool IsDirty { get; set; }

        public DateTime LastMessageDate { get; set; } // Use it for removing out

        public ChatSession ToSession(bool requiredMessageId = false)
        {
            var chatSession = new ChatSession
            {
                Id = SessionId,
                ChatRoomId = ChatRoomId,
                Conversations = new List<Conversation>
                {   
                },
                CreatedDate = CreatedDate,
                LeaveDate = LeaveDate,
                NextSessionId = NextSessionId,
                PreviousSessionId = PreviousSessionId
            };

            if(Messages.Count > 0)
            {
                foreach (var message in Messages)
                {
                    if (requiredMessageId && string.IsNullOrEmpty(message.Id))
                    {
                        message.Id = DataUtil.GenerateUniqueId();
                    }
                    var conversation = new Conversation
                    {
                        Id = message.Id,
                        ChatSessionId = chatSession.Id,
                        CreatedDate = message.CreatedDate,
                        FileUrl = ConvertUtil.SerializeObject(message.AttachmentFiles),
                        Message = message.Message,
                        MessageTransform = message.FormattedMessage,
                        Timestamp = message.TimeStamp,
                        Username = message.UserName
                    };

                    chatSession.Conversations.Add(conversation);
                }   
            }              

            return chatSession;
        }

        public static ChatSessionModel LoadFrom(ChatSession chatSession)
        {
            if(chatSession == null)
            {
                return null;
            }
            var newSession = new ChatSessionModel
            {
                ChatRoomId = chatSession.ChatRoomId,
                Messages = new Queue<MessageModel>(),
                PreviousSessionId = chatSession.PreviousSessionId,
                CreatedDate = chatSession.CreatedDate,
                LeaveDate = chatSession.LeaveDate,
                NextSessionId = chatSession.NextSessionId,
                SessionId = chatSession.Id
            };

            if (chatSession.Conversations != null)
            {
                foreach (var message in chatSession.Conversations.OrderBy(a => a.Timestamp))
                {
                    newSession.Messages.Enqueue(new MessageModel
                    {
                        Message = message.Message,
                        FormattedMessage = message.MessageTransform,
                        TimeStamp = message.Timestamp,
                        UserName = message.Username,
                        CreatedDate = message.CreatedDate,
                        AttachmentFiles = !string.IsNullOrEmpty(message.FileUrl) 
                            ? ConvertUtil.DeserializeObject<List<AttachmentFile>>(message.FileUrl) 
                                : new List<AttachmentFile>()
                    });
                }
            }

            return newSession;
        }
    }

    public class MessageModel
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Message { get; set; }

        public string FormattedMessage { get; set; }

        public List<AttachmentFile> AttachmentFiles { get; set; }

        public long TimeStamp { get; set; }

        public DateTime CreatedDate { get; set; }
    }

    public class AttachmentFile
    {
        public string DownloadUrl { get; set; }

        public string FileType { get; set; }

        public string FileName { get; set; }
    }
}
