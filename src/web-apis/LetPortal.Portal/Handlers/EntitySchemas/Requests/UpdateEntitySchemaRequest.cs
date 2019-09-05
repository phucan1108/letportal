using LetPortal.Portal.Handlers.EntitySchemas.Commands;
using MediatR;

namespace LetPortal.Portal.Handlers.EntitySchemas.Requests
{
    public class UpdateEntitySchemaRequest : IRequest
    {
        private readonly UpdateEntitySchemaCommand _updateEntitySchemaCommand;

        public UpdateEntitySchemaRequest(UpdateEntitySchemaCommand updateEntitySchemaCommand)
        {
            _updateEntitySchemaCommand = updateEntitySchemaCommand;
        }

        public UpdateEntitySchemaCommand GetCommand()
        {
            return _updateEntitySchemaCommand;
        }
    }
}
