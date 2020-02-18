using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.EntitySchemas;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

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

        public async Task UpsertEntitySchemasAsync(
            IEnumerable<EntitySchema> entitySchemas, 
            string databaseId,
            bool isKeptSameName = false)
        {
            foreach (var entitySchema in entitySchemas)
            {
                var isExisted = Collection.AsQueryable()
                        .Any(a => a.Name == entitySchema.Name && a.DatabaseId == databaseId);
                if ((isExisted && isKeptSameName) == false)
                {
                    entitySchema.Id = DataUtil.GenerateUniqueId();
                    entitySchema.DatabaseId = databaseId;
                    await AddAsync(entitySchema);
                }
            }
        }
    }
}
