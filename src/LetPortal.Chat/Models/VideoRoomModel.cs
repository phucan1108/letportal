using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Chat.Models
{
    public class VideoRoomModel
    {
        public string Id { get; set; }

        public List<ParticipantVideoModel> Participants { get; set; } = new List<ParticipantVideoModel>();

        public bool IsConnectedRtc { get; set; }

        public bool IsStoppped { get; set; }

        public DateTime HandshakeDate { get; set; }
        
        public DateTime DroppedDate { get; set; }
    }

    public class ParticipantVideoModel
    {
        public string Username { get; set; }

        public string ConnectionId { get; set; }
    }
}
