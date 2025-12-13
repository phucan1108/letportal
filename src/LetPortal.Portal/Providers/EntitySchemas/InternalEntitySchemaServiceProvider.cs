using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Portal.Entities.EntitySchemas;
using LetPortal.Portal.Repositories.EntitySchemas;

namespace LetPortal.Portal.Providers.EntitySchemas
{
    public class InternalEntitySchemaServiceProvider : IEntitySchemaServiceProvider
    {
        private readonly IEntitySchemaRepository _entitySchemaRepository;

        public InternalEntitySchemaServiceProvider(IEntitySchemaRepository entitySchemaRepository)
        {
            _entitySchemaRepository = entitySchemaRepository;
        }
        public Task<List<EntitySchema>> GetEntitySchemasByAppId(string appId)
        {
            return Task.FromResult(_entitySchemaRepository.GetAsQueryable().Where(a => a.AppId == appId).ToList());
        }
    }
}
