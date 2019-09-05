using LetPortal.Portal.Handlers.EntitySchemas.Commands;
using MediatR;

namespace LetPortal.Portal.Handlers.EntitySchemas.Requests
{
    public class DeleteEntitySchemaRequest : IRequest
    {
        private readonly DeleteEntitySchemaCommand _deleteEntitySchemaCommand;

        public DeleteEntitySchemaRequest(DeleteEntitySchemaCommand deleteEntitySchemaCommand)
        {
            _deleteEntitySchemaCommand = deleteEntitySchemaCommand;
        }

        public DeleteEntitySchemaCommand GetCommand()
        {
            return _deleteEntitySchemaCommand;
        }
    }
}
