using System.Collections.Generic;

namespace LetPortal.Portal.Entities.Menus
{
    public class Menu
    {
        public string Id { get; set; }

        public string DisplayName { get; set; }

        public string Icon { get; set; }

        public string Url { get; set; }

        public int Order { get; set; }

        public string ParentId { get; set; }

        public string MenuPath { get; set; }

        public bool Hide { get; set; }

        public List<Menu> SubMenus { get; set; }
    }
}
