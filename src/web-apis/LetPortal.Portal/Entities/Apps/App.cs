using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Menus;
using System;
using System.Collections.Generic;

namespace LetPortal.Portal.Entities.Apps
{
    [EntityCollection(Name = "apps")]
    public class App : BackupableEntity
    {                                         
        public string Logo { get; set; }

        public string DefaultUrl { get; set; }

        public string Author { get; set; }

        public string CurrentVersionNumber { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

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
