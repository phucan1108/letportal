using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Apps;
using LetPortal.Portal.Entities.Menus;
using LetPortal.Portal.Entities.Shared;
using LetPortal.Portal.Models.Shared;
using Microsoft.EntityFrameworkCore;

namespace LetPortal.Portal.Repositories.Apps
{
    public class AppEFRepository : EFGenericRepository<App>, IAppRepository
    {
        private readonly PortalDbContext _context;

        public AppEFRepository(PortalDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<string> CloneAsync(string cloneId, string cloneName)
        {
            var cloneApp = await _context.Apps.AsNoTracking().FirstAsync(a => a.Id == cloneId);

            cloneApp.Id = DataUtil.GenerateUniqueId();
            cloneApp.Name = cloneName;
            cloneApp.DisplayName += " Clone";
            await AddAsync(cloneApp);

            return cloneApp.Id;
        }

        public async Task<IEnumerable<LanguageKey>> CollectAllLanguages(string appId)
        {
            var app = await GetOneAsync(appId);
            var languages = new List<LanguageKey>();

            languages.AddRange(GetLanguageKeys(app));

            return languages;
        }

        public async Task<IEnumerable<LanguageKey>> GetLanguageKeys(string appId)
        {
            var app = await GetOneAsync(appId);

            return GetLanguageKeys(app);
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

        private IEnumerable<LanguageKey> GetLanguageKeys(App app)
        {
            var languages = new List<LanguageKey>();

            var appName = new LanguageKey
            {
                Key = $"apps.{app.Name}.displayName",
                Value = app.DisplayName
            };

            languages.Add(appName);

            if (app.Menus != null && app.Menus.Count > 0)
            {
                var index = 0;
                foreach (var menu in app.Menus)
                {
                    var menuName = new LanguageKey
                    {
                        Key = $"apps.{app.Name}.menus[{index.ToString()}].displayName",
                        Value = menu.DisplayName
                    };

                    languages.Add(menuName);
                    if (menu.SubMenus != null && menu.SubMenus.Count > 0)
                    {
                        var subIndex = 0;
                        foreach (var subMenu in menu.SubMenus)
                        {
                            var subMenuName = new LanguageKey
                            {
                                Key = $"apps.{app.Name}.menus[{index.ToString()}][{subIndex.ToString()}].displayName",
                                Value = subMenu.DisplayName
                            };
                            languages.Add(subMenuName);
                            subIndex++;
                        }
                    }

                    index++;
                }
            }

            return languages;
        }
    }
}
