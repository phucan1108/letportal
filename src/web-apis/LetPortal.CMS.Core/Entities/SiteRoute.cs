using System.Collections.Generic;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;

namespace LetPortal.CMS.Core.Entities
{
    [EntityCollection(Name = "siteroutes")]
    public class SiteRoute : Entity
    {
        /// <summary>
        /// Ex: /, /home, /blogs
        /// </summary>
        public string RoutePath { get; set; }

        /// <summary>
        /// Specific last elem path, such as 'home', 'blogs', '*'
        /// Ex: Home, Blogs, *
        /// </summary>
        public string ElemPath { get; set; }

        /// <summary>
        /// Use this key to get in SiteContext.RouteValues
        /// </summary>
        public string WildcardKey { get; set; }

        public IList<ResolveDataMap> ResolveMaps { get; set; }

        /// <summary>
        /// 'default' or 'en-Us'
        /// </summary>
        public string LocaleId { get; set; }

        public string PageId { get; set; }

        public string SiteId { get; set; }

        public string ParentId { get; set; }

        public ResponseCaching ResponseCaching { get; set; }
    }

    public class ResponseCaching
    {
        public bool Enable { get; set; }

        public string CacheControl { get; set; }

        /// <summary>
        /// In seconds
        /// </summary>
        public int Duration { get; set; }
    }

    public class ResolveDataMap
    {
        /// <summary>
        /// Key of resolved data
        /// Ex: blog
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Resolver will be executed per key.
        /// Ex: blogResolver
        /// </summary>
        public string Resolver { get; set; }

        /// <summary>
        /// If resolver requires input, you can gather all wildcard keys.
        /// Ex: we have path /categories/*/* -> /categories/{category}/{product}
        /// Then Inputs = category,product -> In pipeline, LetPortal will pass two wildcard values
        /// Also we support '=' to help you to get a correct routedata
        /// </summary>
        public string Inputs { get; set; }

        /// <summary>
        /// Allow to cache In-Memory, it is very dangerours if the data is very huge
        /// Consider before enbable this mode
        /// </summary>
        public bool EnableInMemoryCache { get; set; }

        /// <summary>
        /// Cache Duration, in second
        /// </summary>
        public int CacheDuration { get; set; }
    }
}
