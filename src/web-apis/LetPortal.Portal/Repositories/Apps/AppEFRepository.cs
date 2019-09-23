using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Apps;
using LetPortal.Portal.Entities.Menus;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task UpdateMenuAsync(string appId, List<Menu> menus)
        {
            var app = await _context.Apps.FirstAsync(a => a.Id == appId);
            app.Menus = menus;

            _context.SaveChanges();
        }

        public async Task UpdateMenuProfileAsync(string appId, MenuProfile menuProfile)
        {
            var app = await _context.Apps.FirstAsync(a => a.Id == appId);
            if(app.MenuProfiles == null)
            {
                app.MenuProfiles = new List<MenuProfile>();
            }
            app.MenuProfiles.Add(menuProfile);

            _context.SaveChanges();
        }
    }
}
