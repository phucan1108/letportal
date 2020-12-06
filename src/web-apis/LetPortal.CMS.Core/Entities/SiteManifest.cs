using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;

namespace LetPortal.CMS.Core.Entities
{
    /// <summary>
    /// SiteManifest is very different ThemeManifest
    /// By default, Theme can share SiteManifest by Key, it will reduce your time
    /// </summary>
    [EntityCollection(Name = "sitemanifests")]
    public class SiteManifest : Entity
    {
        public string SiteId { get; set; }

        public string Key { get; set; }

        public string ConfigurableValue { get; set; }

        public ManifestEditorType EditorType { get; set; } = ManifestEditorType.Textbox;
    }


    public enum ManifestEditorType
    {
        Textbox,
        Number,
        Email,
        DatePicker,
        RichTextEditor,
        MediaEditor,
        KeyValueEditor,
        LinkEditor,
        JsonEditor
    }
}
