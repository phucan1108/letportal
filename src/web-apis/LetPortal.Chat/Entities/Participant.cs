using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;

namespace LetPortal.Chat.Entities
{
    [EntityCollection(Name = "participants")]
    [Table("participants")]
    public class Participant : Entity
    {
        [StringLength(50)]
        public string ChatRoomId { get; set; }

        [StringLength(250)]
        public string Username { get; set; }

        public DateTime JoinedDate { get; set; }

        public ChatRoom ChatRoom { get; set; }
    }
}
