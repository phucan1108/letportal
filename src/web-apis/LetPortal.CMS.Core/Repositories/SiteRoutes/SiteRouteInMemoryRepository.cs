using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Entities;
using LetPortal.Core.Persistences;
using Microsoft.Extensions.Caching.Memory;

namespace LetPortal.CMS.Core.Repositories.SiteRoutes
{
    public class SiteRouteInMemoryRepository : InMemoryGenericRepository<SiteRoute>, ISiteRouteRepository
    {
        protected override string CacheKey => "siteroutes";

        public SiteRouteInMemoryRepository(IMemoryCache memoryCache)
            : base(memoryCache)
        {
            List = new List<SiteRoute>
            {
                new SiteRoute
                {
                    Id = "1",
                    ElemPath = "/",
                    PageId = "1",
                    LocaleId = "default",
                    ParentId = "1",
                    RoutePath = "/",
                    SiteId = "1"
                },
                new SiteRoute
                {
                    Id = "2",
                    ElemPath = "TheFace",
                    PageId = "1",
                    LocaleId = "default",
                    ParentId = "1",
                    RoutePath = "/TheFace",
                    SiteId = "1"
                }
            };
        }

        public async Task<IEnumerable<SiteRoute>> GetRoutesSiteAsync(string siteId)
        {
            return await GetAllAsync(a => a.SiteId == siteId);
        }
    }
}
