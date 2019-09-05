using LetPortal.Portal.Handlers.Datasources.Commands;
using MediatR;

namespace LetPortal.Portal.Handlers.Datasources.Requests
{
    public class DeleteDatasourceRequest : IRequest
    {
        private readonly DeleteDatasourceCommand _deleteDatasourceCommand;

        public DeleteDatasourceRequest(DeleteDatasourceCommand deleteDatasourceCommand)
        {
            _deleteDatasourceCommand = deleteDatasourceCommand;
        }

        public DeleteDatasourceCommand GetCommand()
        {
            return _deleteDatasourceCommand;
        }
    }
}
