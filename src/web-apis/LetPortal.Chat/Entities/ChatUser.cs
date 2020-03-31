using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;

namespace LetPortal.Chat.Entities
{
    [EntityCollection(Name = "chatusers")]
    [Table("chatusers")]
    public class ChatUser : Entity
    {
        public string UserName { get; set; }

        public string FullName { get; set; }

        public string Avatar { get; set; }
        
        public bool Deactivate { get; set; }

        public DateTime ActivatedDate { get; set; }

        public DateTime DeactivatedDate { get; set; }
    }
}
