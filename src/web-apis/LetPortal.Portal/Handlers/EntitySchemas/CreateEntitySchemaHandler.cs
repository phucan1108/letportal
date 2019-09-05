using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.EntitySchemas;
using LetPortal.Portal.Handlers.EntitySchemas.Requests;
using LetPortal.Portal.Repositories.EntitySchemas;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.EntitySchemas
{
    public class CreateEntitySchemaHandler : IRequestHandler<CreateEntitySchemaRequest, EntitySchema>
    {
        private readonly IEntitySchemaRepository _entitySchemaRepository;

        public CreateEntitySchemaHandler(IEntitySchemaRepository entitySchemaRepository)
        {
            _entitySchemaRepository = entitySchemaRepository;
        }

        public async Task<EntitySchema> Handle(CreateEntitySchemaRequest request, CancellationToken cancellationToken)
        {
            var insertingEntitySchema = request.GetCommand().EntitySchema;

            insertingEntitySchema.Id = DataUtil.GenerateUniqueId();

            await _entitySchemaRepository.AddAsync(insertingEntitySchema);

            return insertingEntitySchema;
        }
    }
}
