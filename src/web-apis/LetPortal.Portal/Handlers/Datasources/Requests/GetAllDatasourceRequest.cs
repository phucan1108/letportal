using LetPortal.Portal.Entities.Datasources;
using LetPortal.Services.Databases.Handlers.Datasources.Queries;
using MediatR;
using System.Collections.Generic;

namespace LetPortal.Portal.Handlers.Datasources.Requests
{
    public class GetAllDatasourceRequest : IRequest<List<Datasource>>
    {
        private readonly GetAllDatasourceQuery _getAllDatasourceQuery;

        public GetAllDatasourceRequest(GetAllDatasourceQuery getAllDatasourceQuery)
        {
            _getAllDatasourceQuery = getAllDatasourceQuery;
        }

        public GetAllDatasourceQuery GetQuery()
        {
            return _getAllDatasourceQuery;
        }
    }
}
