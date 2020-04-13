using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;
using LetPortal.Portal.Entities.Menus;

namespace LetPortal.Portal.Entities.Apps
{
    [EntityCollection(Name = "apps")]
    [Table("apps")]
    public class App : BackupableEntity
    {
        [StringLength(250)]
        public string Logo { get; set; }

        [StringLength(250)]
        public string DefaultUrl { get; set; }

        [StringLength(250)]
        public string Author { get; set; }

        [StringLength(50)]
        public string CurrentVersionNumber { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        public List<Menu> Menus { get; set; } = new List<Menu>();

        public List<MenuProfile> MenuProfiles { get; set; } = new List<MenuProfile>();
    }

    public class MenuProfile
    {
        public string Role { get; set; }

        public List<string> MenuIds { get; set; }
    }

    [EntityCollection(Name = "appversions")]
    public class AppVersion : Entity
    {
        public string AppId { get; set; }

        public string VersionNumber { get; set; }

        public string Author { get; set; }

        public DateTime CreatedDate { get; set; }

        public string HashKey { get; set; }

        public string CompressionData { get; set; }
    }
}
