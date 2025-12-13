using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Chat.Models
{
    public class SendMessageModel
    {
        public string ChatRoomId { get; set; }
        public string ChatSessionId { get; set; }
        public string Receiver { get; set; }
        public MessageModel Message { get; set; }

        public string LastSentHashCode { get; set; }

        public bool RenderTime { get; set; }
    }
}
