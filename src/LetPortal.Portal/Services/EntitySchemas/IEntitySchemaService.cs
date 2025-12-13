using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Portal.Entities.EntitySchemas;

namespace LetPortal.Portal.Services.EntitySchemas
{
    public interface IEntitySchemaService
    {
        Task<IEnumerable<EntitySchema>> FetchAllEntitiesFromDatabase(string databaseId);
    }
}
