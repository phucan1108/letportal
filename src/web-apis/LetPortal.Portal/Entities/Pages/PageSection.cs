using System.Collections.Generic;

namespace LetPortal.Portal.Entities.Pages
{
    public class PageSection
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public SectionContructionType ConstructionType { get; set; }

        /// <summary>
        /// Note: this shell options support only for Dynamic List or any special section part
        /// </summary>
        public List<ShellOption> OverrideOptions { get; set; }

        public SectionDatasource SectionDatasource { get; set; }

        public string ComponentId { get; set; }

        public int Order { get; set; }
    }

    public class SectionDatasource
    {
        public string DatasourceBindName { get; set; }
    }

    public enum SectionContructionType
    {
        Standard,
        Array,
        DynamicList,
        Chart
    }
}
