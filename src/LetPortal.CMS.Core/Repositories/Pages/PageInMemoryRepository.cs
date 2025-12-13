using System.Collections.Generic;
using LetPortal.CMS.Core.Entities;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using Microsoft.Extensions.Caching.Memory;

namespace LetPortal.CMS.Core.Repositories.Pages
{
    public class PageInMemoryRepository : InMemoryGenericRepository<Page>, IPageRepository
    {
        protected override string CacheKey => "pages";

        public PageInMemoryRepository(IMemoryCache memoryCache)
            : base(memoryCache)
        {
            List = new List<Page>
            {
              new Page
              {
                  Id = "1",
                  Name = "Trang chủ",
                  GoogleMetadata = new GoogleMetadata
                  {
                      Title = "Tali Beauty - Trang chủ",
                      Description = "Trang chính thức của Tali Beauty - Mỹ phẩm làm đẹp cho mọi nhà",
                      AllowGoogleRead = true
                  }
              }
            };
        }
    }
}
