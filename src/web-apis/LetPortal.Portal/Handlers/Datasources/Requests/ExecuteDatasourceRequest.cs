using LetPortal.Portal.Handlers.Datasources.Queries;
using LetPortal.Portal.Models;
using MediatR;

namespace LetPortal.Portal.Handlers.Datasources.Requests
{
    public class ExecuteDatasourceRequest : IRequest<ExecutedDataSourceModel>
    {
        private readonly ExecuteDatasourceQuery _executeDatasourceQuery;

        public ExecuteDatasourceRequest(ExecuteDatasourceQuery executeDatasourceQuery)
        {
            _executeDatasourceQuery = executeDatasourceQuery;
        }

        public ExecuteDatasourceQuery GetQuery()
        {
            return _executeDatasourceQuery;
        }
    }
}
