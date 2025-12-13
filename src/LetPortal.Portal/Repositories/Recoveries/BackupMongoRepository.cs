using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Recoveries;

namespace LetPortal.Portal.Repositories.Recoveries
{
    public class BackupMongoRepository : MongoGenericRepository<Backup>, IBackupRepository
    {
        public BackupMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }
    }
}
