using LetPortal.Portal.Entities.EntitySchemas;
using LetPortal.Portal.Handlers.EntitySchemas.Requests;
using LetPortal.Portal.Repositories.EntitySchemas;
using MediatR;
using MongoDB.Driver;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.EntitySchemas
{
    public class GetOneEntitySchemaByNameHandler : IRequestHandler<GetOneEntitySchemaByNameRequest, EntitySchema>
    {
        private readonly IEntitySchemaRepository _entitySchemaRepository;

        public GetOneEntitySchemaByNameHandler(IEntitySchemaRepository entitySchemaRepository)
        {
            _entitySchemaRepository = entitySchemaRepository;
        }

        public Task<EntitySchema> Handle(GetOneEntitySchemaByNameRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_entitySchemaRepository.GetAsQueryable().FirstOrDefault(a => a.Name == request.GetQuery().EntityName && a.DatabaseId == request.GetQuery().DatabaseId));
        }
    }
}
