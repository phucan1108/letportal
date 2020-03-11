using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Apps;
using LetPortal.Portal.Entities.Menus;
using LetPortal.Portal.Models.Shared;
using Microsoft.EntityFrameworkCore;

namespace LetPortal.Portal.Repositories.Apps
{
    public class AppEFRepository : EFGenericRepository<App>, IAppRepository
    {
        private readonly LetPortalDbContext _context;

        public AppEFRepository(LetPortalDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task CloneAsync(string cloneId, string cloneName)
        {
            var cloneApp = await _context.Apps.AsNoTracking().FirstAsync(a => a.Id == cloneId);

            cloneApp.Id = DataUtil.GenerateUniqueId();
            cloneApp.Name = cloneName;
            cloneApp.DisplayName += " Clone";
            await AddAsync(cloneApp);
        }

        public async Task<IEnumerable<ShortEntityModel>> GetShortApps(string keyWord = null)
        {
            if (!string.IsNullOrEmpty(keyWord))
            {

                var shortApps = await _context.Apps.Where(a => a.DisplayName.Contains(keyWord)).Select(b => new ShortEntityModel { Id = b.Id, DisplayName = b.DisplayName }).ToListAsync();

                return shortApps;
            }
            else
            {
                var shortApps = await _context.Apps.Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName }).ToListAsync();
                return shortApps.AsEnumerable();
            }
        }

        public async Task UpdateMenuAsync(string appId, List<Menu> menus)
        {
            var app = await _context.Apps.FirstAsync(a => a.Id == appId);
            app.Menus = menus;

            _context.SaveChanges();
        }

        public async Task UpdateMenuProfileAsync(string appId, MenuProfile menuProfile)
        {
            var app = await _context.Apps.FirstAsync(a => a.Id == appId);
            if (app.MenuProfiles == null)
            {
                app.MenuProfiles = new List<MenuProfile>();
            }
            app.MenuProfiles.Add(menuProfile);

            _context.SaveChanges();
        }
    }
}
