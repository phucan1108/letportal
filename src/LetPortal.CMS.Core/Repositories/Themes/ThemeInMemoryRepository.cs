using System;
using System.Collections.Generic;
using System.Text;
using LetPortal.CMS.Core.Entities;
using LetPortal.Core.Persistences;
using Microsoft.Extensions.Caching.Memory;

namespace LetPortal.CMS.Core.Repositories.Themes
{
    public class ThemeInMemoryRepository : InMemoryGenericRepository<Theme>, IThemeRepository
    {
        protected override string CacheKey => "themes";

        public ThemeInMemoryRepository(IMemoryCache memoryCache)
            : base(memoryCache)
        {
            List = new List<Theme>
            {
               new Theme
               {
                   Id = "1",
                   Name = "TheFace"
               }
            };
        }
    }
}
