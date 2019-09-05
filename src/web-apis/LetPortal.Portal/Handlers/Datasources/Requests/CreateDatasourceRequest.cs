using LetPortal.Portal.Entities.Datasources;
using LetPortal.Portal.Handlers.Datasources.Commands;
using MediatR;

namespace LetPortal.Portal.Handlers.Datasources.Requests
{
    public class CreateDatasourceRequest : IRequest<Datasource>
    {
        private readonly CreateDatasourceCommand _createDatasourceCommand;

        public CreateDatasourceRequest(CreateDatasourceCommand createDatasourceCommand)
        {
            _createDatasourceCommand = createDatasourceCommand;
        }

        public CreateDatasourceCommand GetCommand()
        {
            return _createDatasourceCommand;
        }
    }
}
