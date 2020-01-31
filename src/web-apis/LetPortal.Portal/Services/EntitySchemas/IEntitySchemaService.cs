using LetPortal.Portal.Entities.EntitySchemas;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetPortal.Portal.Services.EntitySchemas
{
    public interface IEntitySchemaService
    {
        Task<IEnumerable<EntitySchema>> FetchAllEntitiesFromDatabase(string databaseId);
    }
}
