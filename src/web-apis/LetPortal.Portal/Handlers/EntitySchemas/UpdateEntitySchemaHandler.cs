using LetPortal.Portal.Handlers.EntitySchemas.Requests;
using LetPortal.Portal.Repositories.EntitySchemas;
using MediatR;

namespace LetPortal.Portal.Handlers.EntitySchemas
{
    public class UpdateEntitySchemaHandler : RequestHandler<UpdateEntitySchemaRequest>
    {
        private readonly IEntitySchemaRepository _entitySchemaRepository;

        public UpdateEntitySchemaHandler(IEntitySchemaRepository entitySchemaRepository)
        {
            _entitySchemaRepository = entitySchemaRepository;
        }

        protected override void Handle(UpdateEntitySchemaRequest request)
        {
            _entitySchemaRepository.UpdateAsync(request.GetCommand().EntitySchema.Id, request.GetCommand().EntitySchema).Wait();
        }
    }
}
