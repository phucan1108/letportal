using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;

namespace LetPortal.Chat.Entities
{
    [EntityCollection(Name = "chatsessions")]
    [Table("chatsessions")]
    public class ChatSession : Entity
    {
        [StringLength(50)]
        public string ChatRoomId { get; set; }

        [StringLength(50)]
        public string PreviousSessionId { get; set; }

        [StringLength(50)]
        public string NextSessionId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime LeaveDate { get; set; }

        public ChatRoom ChatRoom { get; set; }

        public IList<Conversation> Conversations { get; set; }

    }
}
