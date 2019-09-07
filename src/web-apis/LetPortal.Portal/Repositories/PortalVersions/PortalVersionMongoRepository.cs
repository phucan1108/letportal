using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Versions;

namespace LetPortal.Portal.Repositories.PortalVersions
{
    public class PortalVersionMongoRepository : MongoGenericRepository<PortalVersion>, IPortalVersionRepository
    {
        public PortalVersionMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }
    }
}
