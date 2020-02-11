using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;

namespace LetPortal.Identity.Entities
{
    [EntityCollection(Name = "users")]
    [Table("users")]
    public class User : Entity
    {
        public string Username { get; set; }

        public string NormalizedUserName { get; set; }

        public string Domain { get; set; }

        public string PasswordHash { get; set; }

        public string Email { get; set; }

        public string NormalizedEmail { get; set; }

        public bool IsConfirmedEmail { get; set; }

        public string SecurityStamp { get; set; }

        public int AccessFailedCount { get; set; }

        public bool IsLockoutEnabled { get; set; }

        public DateTime LockoutEndDate { get; set; }

        public List<string> Roles { get; set; } = new List<string>();

        public List<BaseClaim> Claims { get; set; } = new List<BaseClaim>();
    }
}
