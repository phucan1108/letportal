using System.Collections.Generic;
using LetPortal.CMS.Core.Entities;

namespace LetPortal.CMS.Core.Models
{
    public class SiteRouteCache
    {
        public string SiteId { get; set; }

        public string LocaleId { get; set; }

        public SiteRouteMapCache Root { get; set; }
    }

    public class SiteRouteMapCache
    {
        public string Path { get; set; }

        public string ElemPath { get; set; }

        public string WildcardKey { get; set; }

        public IList<ResolveDataMap> ResolveMaps { get; set; }

        public string PageId { get; set; }

        public SiteRouteMapCache Parent { get; set; }

        public List<SiteRouteMapCache> SubRoutes { get; set; }

        public ResponseCaching ResponseCaching { get; set; }

        public Dictionary<string, string> LoadRouteValues(string requestPath)
        {
            var newRouteValues = new Dictionary<string, string>();

            var pathSplitted = requestPath.Split("/");

            var currentRoute = this;
            var startIndex = pathSplitted.Length - 1;
            // Loop back until reach root
            while(currentRoute.Parent != null && startIndex > 1)
            {
                if(currentRoute.ElemPath == "*")
                {
                    newRouteValues.Add(currentRoute.WildcardKey, pathSplitted[startIndex]);
                }
                else
                {
                    newRouteValues.Add(startIndex.ToString(), pathSplitted[startIndex]);
                }
                currentRoute = currentRoute.Parent;
                startIndex--;
            }

            return newRouteValues;
        }
    }
}
