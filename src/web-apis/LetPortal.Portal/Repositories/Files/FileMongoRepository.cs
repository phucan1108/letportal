using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Files;

namespace LetPortal.Portal.Repositories.Files
{
    public class FileMongoRepository : MongoGenericRepository<File>, IFileRepository
    {
        public FileMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }
    }
}
