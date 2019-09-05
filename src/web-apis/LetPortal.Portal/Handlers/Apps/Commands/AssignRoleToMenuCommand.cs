using LetPortal.Portal.Entities.Apps;

namespace LetPortal.Portal.Handlers.Apps.Commands
{
    public class AssignRoleToMenuCommand
    {
        public MenuProfile MenuProfile { get; set; }

        public string AppId { get; set; }
    }
}
