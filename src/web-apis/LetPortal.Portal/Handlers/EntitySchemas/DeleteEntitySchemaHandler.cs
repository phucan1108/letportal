using LetPortal.Portal.Handlers.EntitySchemas.Requests;
using LetPortal.Portal.Repositories.EntitySchemas;
using MediatR;

namespace LetPortal.Portal.Handlers.EntitySchemas
{
    public class DeleteEntitySchemaHandler : RequestHandler<DeleteEntitySchemaRequest>
    {
        private readonly IEntitySchemaRepository _entitySchemaRepository;

        public DeleteEntitySchemaHandler(IEntitySchemaRepository entitySchemaRepository)
        {
            _entitySchemaRepository = entitySchemaRepository;
        }

        protected override void Handle(DeleteEntitySchemaRequest request)
        {
            _entitySchemaRepository.DeleteAsync(request.GetCommand().EntitySchemaId).Wait();
        }
    }
}
