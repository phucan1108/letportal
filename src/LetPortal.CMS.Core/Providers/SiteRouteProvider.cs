using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Entities;
using LetPortal.CMS.Core.Exceptions;
using LetPortal.CMS.Core.Models;
using LetPortal.CMS.Core.Repositories.SiteRoutes;
using Microsoft.Extensions.Caching.Memory;

namespace LetPortal.CMS.Core.Providers
{
    public class SiteRouteProvider : ISiteRouteProvider
    {
        private readonly ISiteRouteRepository _siteRouteRepository;

        private readonly IMemoryCache _memoryCache;

        const string SITE_ROUTE_CACHE = "SITE_ROUTE_CACHE";

        public SiteRouteProvider(
            ISiteRouteRepository siteRouteRepository,
            IMemoryCache memoryCache)
        {
            _siteRouteRepository = siteRouteRepository;
            _memoryCache = memoryCache;
        }

        public async Task<SiteRouteMapCache> FindRouteAsync(string siteId, string localeId, string path)
        {
            var siteRouteCacheName = SITE_ROUTE_CACHE + "_" + siteId;
            if (_memoryCache.TryGetValue(siteRouteCacheName, out List<SiteRouteCache> siteRouteCache))
            {
                var siteRoute = siteRouteCache.FirstOrDefault(a => a.LocaleId == localeId);
                if (siteRoute != null)
                    return FindRoute(siteRoute, path);
            }
            else
            {
                // Fetch from Repo
                var siteRoutes = await _siteRouteRepository.GetRoutesSiteAsync(siteId);
                if (siteRoutes.Any())
                {
                    var routeCaches = ConstructRoutes(siteRoutes, siteId);
                    _memoryCache.Set(siteRouteCacheName, routeCaches);
                    var siteRoute = routeCaches.FirstOrDefault(a => a.LocaleId == localeId);
                    if (siteRoute != null)
                        return FindRoute(siteRoute, path);
                }
            }
            throw new CMSException(ErrorCodes.NotFoundSiteRoute);
        }

        private List<SiteRouteCache> ConstructRoutes(IEnumerable<SiteRoute> siteRoutes, string siteId)
        {
            var routeCaches = new List<SiteRouteCache>();

            // Group by LocaleId
            var groupedLocales = siteRoutes.GroupBy(a => a.LocaleId);

            foreach (var groupLocale in groupedLocales)
            {
                var routeCache = new SiteRouteCache
                {
                    LocaleId = groupLocale.Key,
                    SiteId = siteId
                };

                var siteMaps = new List<SiteRouteMapCache>();
                var lookup = new Dictionary<string, SiteRouteMapCache>();
                foreach (var route in groupLocale)
                {
                    if (lookup.TryGetValue(route.Id, out var detectedMap))
                    {
                        // This map has been found as Parent
                        detectedMap.PageId = route.PageId;
                        detectedMap.Path = route.RoutePath;
                        detectedMap.ElemPath = route.ElemPath;
                        detectedMap.ResolveMaps = route.ResolveMaps;
                        detectedMap.WildcardKey = route.WildcardKey;
                        detectedMap.ResponseCaching = route.ResponseCaching;
                    }
                    else
                    {
                        detectedMap = new SiteRouteMapCache
                        {
                            PageId = route.PageId,
                            Path = route.RoutePath,
                            ElemPath = route.ElemPath,
                            WildcardKey = route.WildcardKey,
                            ResolveMaps = route.ResolveMaps,
                            ResponseCaching = route.ResponseCaching,
                            SubRoutes = new List<SiteRouteMapCache>()
                        };
                        lookup.Add(route.Id, detectedMap);
                    }

                    // Root node
                    if (route.ParentId == route.Id)
                    {
                        routeCache.Root = detectedMap;
                    }
                    else
                    {
                        if (!lookup.TryGetValue(route.ParentId, out var parentMap))
                        {
                            // Temp Parent
                            parentMap = new SiteRouteMapCache
                            {
                                SubRoutes = new List<SiteRouteMapCache>()
                            };
                            lookup.Add(route.ParentId, parentMap);
                        }
                        parentMap.SubRoutes.Add(detectedMap);

                        detectedMap.Parent = parentMap;
                    }
                }

                routeCaches.Add(routeCache);
            }

            return routeCaches;
        }

        private SiteRouteMapCache FindRoute(SiteRouteCache siteRoute, string path)
        {
            if (path.Equals("/") || string.IsNullOrEmpty(path))
            {
                return siteRoute.Root;
            }
            else
            {
                var pathSplitted = path.Split("/");
                if (pathSplitted.Length > 1 || (siteRoute.Root.SubRoutes != null && pathSplitted.Length > 2))
                {
                    // Ensure the first elem path must be matched
                    var result = siteRoute.Root.SubRoutes.FirstOrDefault(a => a.ElemPath.ToLower() == pathSplitted[1].ToLower());
                    if(result != null)
                    {
                        for (var i = 2; i < pathSplitted.Length; i++)
                        {
                            // Specific path is higher priority than asterisk
                            var tempRoute = result.SubRoutes.FirstOrDefault(a => a.ElemPath == pathSplitted[i]);

                            if(tempRoute == null)
                            {
                                tempRoute = result.SubRoutes.First(a => a.ElemPath == "*");
                            }

                            result = tempRoute;
                        }
                    }                     

                    return result;
                }
            }
            throw new CMSException(ErrorCodes.NotFoundSiteRoute);
        }
    }
}
