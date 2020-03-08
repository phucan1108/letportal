using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.EntitySchemas;

namespace LetPortal.Portal.Repositories.EntitySchemas
{
    public interface IEntitySchemaRepository : IGenericRepository<EntitySchema>
    {
        Task<EntitySchema> GetOneEntitySchemaAsync(string databaseId, string name);

        Task UpsertEntitySchemasAsync(
            IEnumerable<EntitySchema> entitySchemas,
            string databaseId,
            bool isKeptSameName = false);
    }
}
