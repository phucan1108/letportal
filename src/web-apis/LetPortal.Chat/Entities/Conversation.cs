using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;

namespace LetPortal.Chat.Entities
{
    [EntityCollection(Name = "conversations")]
    [Table("conversations")]
    public class Conversation : Entity
    {
        [StringLength(250)]
        public string Username { get; set; }

        public long Timestamp { get; set; }

        public DateTime CreatedDate { get; set; }

        [StringLength(1000)]
        public string Message { get; set; }

        [StringLength(2000)]
        public string MessageTransform { get; set; }

        [StringLength(2000)]
        public string FileUrl { get; set; }

        [StringLength(50)]
        public string ChatSessionId { get; set; }

        public ChatSession ChatSession { get; set; }
    }
}
