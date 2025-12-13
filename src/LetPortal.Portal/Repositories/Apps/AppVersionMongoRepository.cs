using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Apps;

namespace LetPortal.Portal.Repositories.Apps
{
    public class AppVersionMongoRepository : MongoGenericRepository<AppVersion>, IAppVersionRepository
    {
        public AppVersionMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }
    }
}
