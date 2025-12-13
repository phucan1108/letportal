using System.Collections.Generic;
using LetPortal.CMS.Core.Abstractions;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;

namespace LetPortal.CMS.Core.Entities
{
    [EntityCollection(Name = "pagetemplates")]
    public class PageTemplate : Entity
    {
        public string Name { get; set; }

        public string ThemeId { get; set; }

        public string SiteId { get; set; }

        public List<TemplateSection> Sections { get; set; }
    }

    public class TemplateSection
    {

        /// <summary>
        /// Auto-generated field for unique section
        /// Each template has one or many ThemePart, so unique key is helpful to distinct value
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Each template section has own name in Builder
        /// </summary>
        public string Name { get; set; }

        public BindingType BindingType { get; set; }

        /// <summary>
        /// Allow section is hidden
        /// </summary>
        public bool Hide { get; set; }

        /// <summary>
        /// This is a name of theme part, also effectived when theme is changed
        /// </summary>
        public string ThemePartRef { get; set; }

        /// <summary>
        /// This default items will be copied when reference page got new version
        /// </summary>

        public List<TemplateDefaultItem> Items { get; set; }
    }

    public class TemplateDefaultItem
    {
        /// <summary>
        /// Refer to ThemeManifest Key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Default value to be copied when creating new Page Version
        /// </summary>
        public string Value { get; set; }

        public ManifestEditorType EditorType { get; set; } = ManifestEditorType.Textbox;
    }
}
