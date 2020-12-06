using System.Collections.Generic;
using LetPortal.CMS.Core.Entities;
using LetPortal.CMS.Core.Models;
using LetPortal.CMS.Core.Shared;

namespace LetPortal.CMS.Core
{
    /// <summary>
    /// This class represents all required data for one request as AddScope
    /// Allows to modify any property instead of using Builder pattern to restrict
    /// </summary>
    public sealed class SiteContext
    {
        public Site Site { get; set; }

        public SiteRouteMapCache Route { get; set; }

        /// <summary>
        /// Wildcard values. Ex: { "category": "vga" }, { "product" : "nvidia" }
        /// </summary>
        public Dictionary<string, string> RouteValues { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, string> QueryParams { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// By default, Let Portal will catch QueryString to set Pagination
        /// 'page' => Current Page, null will be 0
        /// 'size' => Numer per page, null will be 10
        /// </summary>
        public Pagination Pagination { get; set; } 

        /// <summary>
        /// Resolved data, set by Resolver
        /// </summary>
        public Dictionary<string, object> ResolvedData { get; set; } = new Dictionary<string, object>();

        public string LocaleId { get; set; }

        public Page Page { get; set; }

        public PageVersion PageVersion { get; set; }

        public PageTemplate PageTemplate { get; set; }

        public Theme Theme { get; set; }

        public FlexiblePageModel RenderedPageModel { get; set; }
    }
}
