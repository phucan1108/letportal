using LetPortal.Portal.Entities.Shared;

namespace LetPortal.Portal.Entities.Pages
{
    public class PageDatasource
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string TriggerCondition { get; set; }

        public bool IsActive { get; set; }

        public DatasourceOptions Options { get; set; }
    }
}
