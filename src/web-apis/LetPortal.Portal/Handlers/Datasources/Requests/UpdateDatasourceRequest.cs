using LetPortal.Portal.Handlers.Datasources.Commands;
using MediatR;

namespace LetPortal.Portal.Handlers.Datasources.Requests
{
    public class UpdateDatasourceRequest : IRequest
    {
        private readonly UpdateDatasourceCommand _updateDatasourceCommand;

        public UpdateDatasourceRequest(UpdateDatasourceCommand updateDatasourceCommand)
        {
            _updateDatasourceCommand = updateDatasourceCommand;
        }

        public UpdateDatasourceCommand GetCommand()
        {
            return _updateDatasourceCommand;
        }
    }
}
