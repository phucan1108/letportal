using LetPortal.Core.Persistences;

namespace LetPortal.Core.Versions
{
    public class VersionMongoRepository : MongoGenericRepository<Version>, IVersionRepository
    {
        public VersionMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }
    }
}
