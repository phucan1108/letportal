using LetPortal.Portal.Entities.Datasources;
using LetPortal.Portal.Handlers.Datasources.Queries;
using MediatR;

namespace LetPortal.Portal.Handlers.Datasources.Requests
{
    public class GetOneDatasourceRequest : IRequest<Datasource>
    {
        private readonly GetOneDatasourceQuery _getOneDatasourceQuery;

        public GetOneDatasourceRequest(GetOneDatasourceQuery getOneDatasourceQuery)
        {
            _getOneDatasourceQuery = getOneDatasourceQuery;
        }

        public GetOneDatasourceQuery GetQuery()
        {
            return _getOneDatasourceQuery;
        }
    }
}
