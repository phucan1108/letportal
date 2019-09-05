using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Apps;
using LetPortal.Portal.Entities.Menus;
using MongoDB.Driver;
using System.Collections.Generic;
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
            var collection = Connection.GetDatabaseConnection().GetCollection<App>(CollectionName);
            var updateBuilder = Builders<App>.Update.Set(a => a.Menus, menus);
            await collection.FindOneAndUpdateAsync(a => a.Id == appId, updateBuilder);
        }
    }
}
