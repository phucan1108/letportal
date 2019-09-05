using LetPortal.Portal.Entities.EntitySchemas;
using LetPortal.Portal.Handlers.EntitySchemas.Requests;
using LetPortal.Portal.Repositories.EntitySchemas;
using MediatR;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.EntitySchemas
{
    public class GetAllEntitySchemasOfOneDatabaseHandler : IRequestHandler<GetAllEntitySchemasOfOneDatabaseRequest, List<EntitySchema>>
    {
        private readonly IEntitySchemaRepository _entitySchemaRepository;

        public GetAllEntitySchemasOfOneDatabaseHandler(IEntitySchemaRepository entitySchemaRepository)
        {
            _entitySchemaRepository = entitySchemaRepository;
        }

        public Task<List<EntitySchema>> Handle(GetAllEntitySchemasOfOneDatabaseRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_entitySchemaRepository.GetAsQueryable().ToList());
        }
    }
}
