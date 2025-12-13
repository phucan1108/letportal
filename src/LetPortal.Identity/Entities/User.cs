using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;

namespace LetPortal.Identity.Entities
{
    [EntityCollection(Name = "users")]
    [Table("users")]
    public class User : Entity
    {
        [StringLength(250)]
        public string Username { get; set; }

        [StringLength(250)]
        public string NormalizedUserName { get; set; }

        [StringLength(250)]
        public string Domain { get; set; }

        [StringLength(250)]
        public string PasswordHash { get; set; }

        [StringLength(250)]
        public string Email { get; set; }

        [StringLength(250)]
        public string NormalizedEmail { get; set; }

        public bool IsConfirmedEmail { get; set; }

        [StringLength(250)]
        public string SecurityStamp { get; set; }

        public int AccessFailedCount { get; set; }

        public bool IsLockoutEnabled { get; set; }

        public DateTime LockoutEndDate { get; set; }

        public List<string> Roles { get; set; } = new List<string>();

        public List<BaseClaim> Claims { get; set; } = new List<BaseClaim>();
    }
}
