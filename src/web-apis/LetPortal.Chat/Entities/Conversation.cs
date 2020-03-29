﻿using System;
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

        [StringLength(5000)]
        public string Message { get; set; }

        [StringLength(10000)]
        public string MessageTransform { get; set; }

        [StringLength(5000)]
        public string FileUrl { get; set; }

        public string ChatSessionId { get; set; }

        public ChatSession ChatSession { get; set; }
    }
}
