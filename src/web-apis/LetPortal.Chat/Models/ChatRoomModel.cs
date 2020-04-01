using System;
using System.Collections.Generic;
using System.Linq;
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

        public static ChatRoomModel Load(ChatRoom chatRoom)
        {
            return new ChatRoomModel
            {
              ChatRoomId = chatRoom.Id,
              Participants = chatRoom.Participants.Select(a => new OnlineUser { UserName = a.Username }).ToList(),
              RoomName = chatRoom.RoomName,
              Type = chatRoom.Type
            };
        }
    }
}
