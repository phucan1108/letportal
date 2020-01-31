using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.EntitySchemas;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetPortal.Portal.Repositories.EntitySchemas
{
    public class EntitySchemaMongoRepository : MongoGenericRepository<EntitySchema>, IEntitySchemaRepository
    {
        public EntitySchemaMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }

        public async Task<EntitySchema> GetOneEntitySchemaAsync(string databaseId, string name)
        {
            return await Collection.AsQueryable().FirstOrDefaultAsync(a => a.Name == name && a.DatabaseId == databaseId);
        }

        public async Task UpsertEntitySchemasAsync(IEnumerable<EntitySchema> entitySchemas, bool isKeptSameName = false)
        {
            foreach(EntitySchema entitySchema in entitySchemas)
            {
                bool isExisted = Collection.AsQueryable().Any(a => a.Name == entitySchema.Name);
                if((isExisted && isKeptSameName) == false)
                {
                    entitySchema.Id = DataUtil.GenerateUniqueId();
                    await AddAsync(entitySchema);
                }
            }
        }
    }
}
