using LetPortal.Portal.Entities.Apps;

namespace LetPortal.Portal.Handlers.Apps.Commands
{
    public class UpdateAppCommand
    {
        public string AppId { get; set; }

        public App App { get; set; }
    }
}
