using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;

namespace LetPortal.Identity.Entities
{
    [EntityCollection(Name = "usersessions")]
    [Table("usersessions")]
    public class UserSession : Entity
    {
        [StringLength(50)]
        public string UserId { get; set; }

        [StringLength(250)]
        public string Username { get; set; }

        public User User { get; set; }

        [StringLength(250)]
        public string InstalledVersion { get; set; }

        [StringLength(250)]
        public string SoftwareAgent { get; set; }

        [StringLength(250)]
        public string RequestIpAddress { get; set; }

        public DateTime SignInDate { get; set; }

        public DateTime SignOutDate { get; set; }

        public bool AlreadySignOut { get; set; }

        public List<UserActivity> UserActivities { get; set; }
    }

    [Table("useractivities")]
    public class UserActivity
    {
        [StringLength(50)]
        public string Id { get; set; }

        [StringLength(250)]
        public string ActivityName { get; set; }

        [StringLength(1000)]
        public string Content { get; set; }

        public ActivityType ActivityType { get; set; }

        public DateTime ActivityDate { get; set; }

        public UserSession UserSession { get; set; }
    }

    public enum ActivityType
    {
        Info,
        Warn,
        Critical
    }
}
