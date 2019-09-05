using LetPortal.Core.Persistences;
using LetPortal.Portal.Constants;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Repositories.Databases;

namespace LetPortal.Portal.Repositories
{
    public class DatabaseMongoRepository : MongoGenericRepository<DatabaseConnection>, IDatabaseRepository
    {
        public DatabaseMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }
    }
}
