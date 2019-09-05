using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Handlers.Databases.Queries;
using MediatR;

namespace LetPortal.Portal.Handlers.Databases.Requests
{
    public class GetOneDatabaseRequest : IRequest<DatabaseConnection>
    {
        private readonly GetOneDatabaseQuery _getOneDatabaseQuery;

        public GetOneDatabaseRequest(GetOneDatabaseQuery getOneDatabaseQuery)
        {
            _getOneDatabaseQuery = getOneDatabaseQuery;
        }

        public GetOneDatabaseQuery GetQuery()
        {
            return _getOneDatabaseQuery;
        }
    }
}
