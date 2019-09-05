using LetPortal.Portal.Entities.EntitySchemas;
using LetPortal.Portal.Handlers.EntitySchemas.Commands;
using MediatR;

namespace LetPortal.Portal.Handlers.EntitySchemas.Requests
{
    public class CreateEntitySchemaRequest : IRequest<EntitySchema>
    {
        private readonly CreateEntitySchemaCommand _createEntitySchemaCommand;

        public CreateEntitySchemaRequest(CreateEntitySchemaCommand createEntitySchemaCommand)
        {
            _createEntitySchemaCommand = createEntitySchemaCommand;
        }

        public CreateEntitySchemaCommand GetCommand()
        {
            return _createEntitySchemaCommand;
        }
    }
}
