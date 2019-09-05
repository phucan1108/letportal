using LetPortal.Portal.Entities.SectionParts;

namespace LetPortal.Portal.Handlers.Components.DynamicLists.Commands
{
    public class UpdateDynamicListCommand
    {
        public string DynamicListId { get; set; }

        public DynamicList DynamicList { get; set; }
    }
}
