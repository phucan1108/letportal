using LetPortal.Portal.Entities.Shared;

namespace LetPortal.Portal.Entities.Components.Controls
{
    public class StandardCommand
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Color { get; set; }

        public bool IsRequiredValidation { get; set; }

        public string AllowHidden { get; set; }

        public ActionCommandOptions ActionCommandOptions { get; set; }
    }
}
