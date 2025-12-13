using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Chat.Models
{
    public class VideoRtcSignalModel
    {
        public string RoomId { get; set; }

        public string SignalMessage { get; set; }

        public string ConnectionId { get; set; }
    }
}
