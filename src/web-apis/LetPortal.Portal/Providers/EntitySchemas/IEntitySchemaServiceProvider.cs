using LetPortal.Portal.Entities.EntitySchemas;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetPortal.Portal.Providers.EntitySchemas
{
    public interface IEntitySchemaServiceProvider
    {
        Task<List<EntitySchema>> GetEntitySchemasByAppId(string appId);
    }
}
