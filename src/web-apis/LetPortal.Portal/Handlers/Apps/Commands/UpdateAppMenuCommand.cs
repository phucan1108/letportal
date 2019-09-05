using LetPortal.Portal.Entities.Menus;
using System.Collections.Generic;

namespace LetPortal.Portal.Handlers.Apps.Commands
{
    public class UpdateAppMenuCommand
    {
        public string AppId { get; set; }

        public List<Menu> Menus { get; set; }
    }
}
