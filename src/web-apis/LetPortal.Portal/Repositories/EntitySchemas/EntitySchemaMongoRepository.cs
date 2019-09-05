using LetPortal.Core.Persistences;
using LetPortal.Portal.Constants;
using LetPortal.Portal.Entities.EntitySchemas;

namespace LetPortal.Portal.Repositories.EntitySchemas
{
    public class EntitySchemaMongoRepository : MongoGenericRepository<EntitySchema>, IEntitySchemaRepository
    {
        public EntitySchemaMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }
    }
}
