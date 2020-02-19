using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;

namespace LetPortal.Identity.Entities
{
    [EntityCollection(Name = "usersessions")]
    [Table("usersessions")]
    public class UserSession : Entity
    {
        public string UserId { get; set; }

        public User User { get; set; }

        public string InstalledVersion { get; set; }

        public string SoftwareAgent { get; set; }

        public string RequestIpAddress { get; set; }

        public DateTime SignInDate { get; set; }

        public DateTime SignOutDate { get; set; }

        public List<UserActivity> UserActivities { get; set; }
    }

    [Table("useractivities")]
    public class UserActivity
    {
        public string Id { get; set; }

        public string ActivityName { get; set; }

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
