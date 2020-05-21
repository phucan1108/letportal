using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Apps;
using LetPortal.Portal.Entities.Menus;
using LetPortal.Portal.Entities.Shared;
using LetPortal.Portal.Models.Shared;
using MongoDB.Driver;

namespace LetPortal.Portal.Repositories.Apps
{
    public class AppMongoRepository : MongoGenericRepository<App>, IAppRepository
    {
        public AppMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }

        public async Task<string> CloneAsync(string cloneId, string cloneName)
        {
            var cloneApp = await GetOneAsync(cloneId);
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

        public Task<IEnumerable<ShortEntityModel>> GetShortApps(string keyWord = null)
        {
            if (!string.IsNullOrEmpty(keyWord))
            {
                var filterBuilder = Builders<App>.Filter.Regex(a => a.DisplayName, new MongoDB.Bson.BsonRegularExpression(keyWord, "i"));

                return Task.FromResult(Collection.Find(filterBuilder).ToList()?.Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName }).AsEnumerable());
            }
            return Task.FromResult(Collection.AsQueryable().Select(a => new ShortEntityModel { Id = a.Id, DisplayName = a.DisplayName }).AsEnumerable());
        }

        public async Task UpdateMenuAsync(string appId, List<Menu> menus)
        {
            var updateBuilder = Builders<App>.Update.Set(a => a.Menus, menus);
            await Collection.FindOneAndUpdateAsync(a => a.Id == appId, updateBuilder);
        }

        public async Task UpdateMenuProfileAsync(string appId, MenuProfile menuProfile)
        {
            var app = await GetOneAsync(appId);
            if (app.MenuProfiles.Any(a => a.Role == menuProfile.Role))
            {
                foreach (var menu in app.MenuProfiles)
                {
                    if (menuProfile.Role == menuProfile.Role)
                    {
                        menuProfile.MenuIds = menuProfile.MenuIds;
                        break;
                    }
                }
            }
            else
            {
                app.MenuProfiles.Add(menuProfile);
            }

            await UpdateAsync(app.Id, app);
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

            if(app.Menus != null && app.Menus.Count > 0)
            {
                var index = 0;
                foreach(var menu in app.Menus)
                {
                    var menuName = new LanguageKey
                    {
                        Key = $"apps.{app.Name}.menus[{index.ToString()}].displayName",
                        Value = menu.DisplayName
                    };

                    languages.Add(menuName);                    
                    if(menu.SubMenus != null && menu.SubMenus.Count > 0)
                    {
                        var subIndex = 0;
                        foreach(var subMenu in menu.SubMenus)
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
