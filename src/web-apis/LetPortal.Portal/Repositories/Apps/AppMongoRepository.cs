using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Apps;
using LetPortal.Portal.Entities.Menus;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetPortal.Portal.Repositories.Apps
{
    public class AppMongoRepository : MongoGenericRepository<App>, IAppRepository
    {
        public AppMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }

        public async Task UpdateMenuAsync(string appId, List<Menu> menus)
        {
            var updateBuilder = Builders<App>.Update.Set(a => a.Menus, menus);
            await Collection.FindOneAndUpdateAsync(a => a.Id == appId, updateBuilder);
        }

        public async Task UpdateMenuProfileAsync(string appId, MenuProfile menuProfile)
        {
            var app = await GetOneAsync(appId);
            if(app.MenuProfiles.Any(a => a.Role == menuProfile.Role))
            {
                foreach(var menu in app.MenuProfiles)
                {
                    if(menuProfile.Role == menuProfile.Role)
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
    }
}
