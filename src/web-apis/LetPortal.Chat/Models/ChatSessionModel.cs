using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Chat.Models
{
    public class ChatSessionModel
    {
        public string SessionId { get; set; }

        public string ChatRoomId { get; set; }

        public Queue<MessageModel> Messages { get; set; }

        public DateTime CreatedDate { get; set; }

        public string PreviousSessionId { get; set; }

        public string NextSessionId { get; set; }
    }

    public class MessageModel
    {
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
