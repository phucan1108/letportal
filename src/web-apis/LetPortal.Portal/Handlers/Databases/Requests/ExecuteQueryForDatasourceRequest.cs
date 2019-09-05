using LetPortal.Portal.Handlers.Databases.Queries;
using LetPortal.Portal.Models;
using MediatR;

namespace LetPortal.Portal.Handlers.Databases.Requests
{
    public class ExecuteQueryForDatasourceRequest : IRequest<ExecuteDynamicResultModel>
    {
        private readonly ExecuteQueryForDatasourceQuery _executeQueryForDatasourceQuery;

        public ExecuteQueryForDatasourceRequest(ExecuteQueryForDatasourceQuery executeQueryForDatasourceQuery)
        {
            _executeQueryForDatasourceQuery = executeQueryForDatasourceQuery;
        }

        public ExecuteQueryForDatasourceQuery GetQuery()
        {
            return _executeQueryForDatasourceQuery;
        }
    }
}
