using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Entities;
using LetPortal.Core.Persistences;
using Microsoft.Extensions.Caching.Memory;

namespace LetPortal.CMS.Core.Repositories.Sites
{
    public class SiteInMemoryRepository : InMemoryGenericRepository<Site>, ISiteRepository
    {
        protected override string CacheKey => "sites";

        public SiteInMemoryRepository(IMemoryCache memoryCache)
            : base(memoryCache)
        {
            List = new List<Site>
            {
              new Site
              {
                  Id = "1",
                  Domains =new List<string> {"localhost:59833" },
                  DefaultLocaleId = "default",
                  DefaultPathWhenNotFound = "/404",
                  Enable = true,
                  Name = "Tali Beauty",
                  SiteMap = new SiteMap(),
                  ThemeId = "1"
              }
            };
        }

        public Task<Site> GetByDomain(string domain)
        {
            return Task.FromResult(List.First(a => a.Domains.Any(b => b == domain)));
        }
    }
}
