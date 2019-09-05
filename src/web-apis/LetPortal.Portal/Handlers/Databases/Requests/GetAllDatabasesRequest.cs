using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Handlers.Databases.Queries;
using MediatR;
using System.Collections.Generic;

namespace LetPortal.Portal.Handlers.Databases.Requests
{
    public class GetAllDatabasesRequest : IRequest<List<DatabaseConnection>>
    {
        private readonly GetAllDatabasesQuery _getAllDatabasesQuery;

        public GetAllDatabasesRequest(GetAllDatabasesQuery getAllDatabasesQuery)
        {
            _getAllDatabasesQuery = getAllDatabasesQuery;
        }

        public GetAllDatabasesQuery GetQuery()
        {
            return _getAllDatabasesQuery;
        }
    }
}
