using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Portal.Entities.EntitySchemas;

namespace LetPortal.Portal.Providers.EntitySchemas
{
    public interface IEntitySchemaServiceProvider
    {
        Task<List<EntitySchema>> GetEntitySchemasByAppId(string appId);
    }
}
