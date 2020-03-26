using LetPortal.Portal.Entities.Shared;

namespace LetPortal.Portal.Entities.Pages
{
    public class PageButton
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public string Color { get; set; }

        public bool IsRequiredValidation { get; set; }

        public string AllowHidden { get; set; }

        public string PlaceSectionId { get; set; }

        public ButtonOptions ButtonOptions { get; set; }
    }
}
