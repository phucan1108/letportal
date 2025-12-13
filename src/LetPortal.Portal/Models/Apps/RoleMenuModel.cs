using System.Collections.Generic;

namespace LetPortal.Portal.Models.Apps
{
    public class RoleMenuModel
    {
        public string MenuId { get; set; }

        public List<string> Roles { get; set; }

        public List<RoleMenuModel> SubMenus { get; set; }
    }
}
