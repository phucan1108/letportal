using System.Collections;
using System.Collections.Generic;

namespace LetPortal.CMS.Core.Entities
{
    /// <summary>
    /// Unlike SiteManifest is fixed value, PageManifest can provide based on version no
    /// And it belongs to Page scope
    /// </summary>
    public class VersionValue
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public ManifestEditorType EditorType { get; set; } = ManifestEditorType.Textbox;
    }

    public class VersionValueList
    {
        public List<VersionValue> Values { get; set; }
    }
}
