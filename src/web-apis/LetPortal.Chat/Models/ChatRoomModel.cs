using System;
using System.Collections.Generic;
using System.Text;
using LetPortal.Chat.Entities;

namespace LetPortal.Chat.Models
{
    public class ChatRoomModel
    {
        public string ChatRoomId { get; set; }

        public RoomType Type { get; set; }

        public string RoomName { get; set; }

        public List<OnlineUser> Participants { get; set; }
    }
}
