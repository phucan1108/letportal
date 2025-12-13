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

        public string Hidden { get; set; } = "false";

        public string Rendered { get; set; } = "true";

        public int Order { get; set; }
    }

    public class SectionDatasource
    {
        /// <summary>
        /// Key of datasource which helps to bind the data into Section
        /// </summary>
        public string DatasourceBindName { get; set; }

        /// <summary>
        /// Key of section data which helps to store in data state.
        /// Note: it doesn't want to declare explicitly 'data'. Ex: 'info' -> 'data.info' will be stored
        /// </summary>
        public string DataStoreName { get; set; }
    }

    public enum SectionContructionType
    {
        Standard,
        Array,
        DynamicList,
        Chart,
        Tree
    }
}
