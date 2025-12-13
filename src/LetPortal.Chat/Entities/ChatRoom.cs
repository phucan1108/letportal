using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;

namespace LetPortal.Chat.Entities
{
    [EntityCollection(Name = "chatrooms")]
    [Table("chatrooms")]
    public class ChatRoom : Entity
    {
        [StringLength(250)]
        public string RoomName { get; set; }        

        public RoomType Type { get; set; }

        public DateTime CreatedDate { get; set; }

        public IList<Participant> Participants { get; set; }

        public IList<ChatSession> Sessions { get; set; }
    }

    public enum RoomType
    {
        Double,
        Group
    }
}
