using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Entities;
using LetPortal.Core.Persistences;
using Microsoft.Extensions.Caching.Memory;

namespace LetPortal.CMS.Core.Repositories.Sites
{
    public class SiteManifestInMemoryRepository : InMemoryGenericRepository<SiteManifest>, ISiteManifestRepository
    {
        protected override string CacheKey => "sitemanifests";

        public SiteManifestInMemoryRepository(IMemoryCache memoryCache)
            : base(memoryCache)
        {
            List = new List<SiteManifest>
            {
            };
        }

        public async Task<List<SiteManifest>> GetSiteManifestsAsync(IEnumerable<string> keys, string siteId)
        {
            return (await GetAllAsync(a => keys.Any(b => b == a.Key) && a.SiteId == siteId)).ToList();
        }
    }
}
