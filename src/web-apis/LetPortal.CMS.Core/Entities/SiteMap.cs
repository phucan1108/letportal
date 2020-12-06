using System.Collections.Generic;

namespace LetPortal.CMS.Core.Entities
{
    public class SiteMap
    {
        public PageMap Root { get; set; }
    }

    public class PageMap
    {
        public string PageId { get; set; }

        public bool VisibleOnBreadcrumb { get; set; }

        public bool Wildcard { get; set; }

        public List<PageMap> Sub { get; set; }
    }
}
