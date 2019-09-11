using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.EntitySchemas;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetPortal.Portal.Repositories.EntitySchemas
{
    public interface IEntitySchemaRepository : IGenericRepository<EntitySchema>
    {
        Task<EntitySchema> GetOneEntitySchemaAsync(string databaseId, string name);

        Task UpsertEntitySchemasAsync(List<EntitySchema> entitySchemas, bool isKeptSameName = false); 
    }
}
