using System.Collections.Generic;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;

namespace LetPortal.CMS.Features.Menus.Entities
{
    [EntityCollection(Name = "menus")]
    public class SiteMenu : Entity
    {
        public string SiteId { get; set; }

        public List<Menu> Menus { get; set; } 
    }

    public class Menu
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public List<Menu> Sub { get; set; }
    }
}
