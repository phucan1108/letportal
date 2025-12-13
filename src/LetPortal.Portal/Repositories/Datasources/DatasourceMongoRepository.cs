using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Datasources;

namespace LetPortal.Portal.Repositories.Datasources
{
    public class DatasourceMongoRepository : MongoGenericRepository<Datasource>, IDatasourceRepository
    {
        public DatasourceMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }
    }
}
