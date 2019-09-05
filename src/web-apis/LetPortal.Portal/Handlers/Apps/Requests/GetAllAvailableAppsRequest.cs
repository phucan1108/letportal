using LetPortal.Portal.Entities.Apps;
using LetPortal.Portal.Handlers.Apps.Queries;
using MediatR;
using System.Collections.Generic;

namespace LetPortal.Portal.Handlers.Apps.Requests
{
    public class GetAllAvailableAppsRequest : IRequest<List<App>>
    {
        private readonly GetAllAvailableAppsQuery _getAllAvailableAppsQuery;

        public GetAllAvailableAppsRequest(GetAllAvailableAppsQuery getAllAvailableAppsQuery)
        {
            _getAllAvailableAppsQuery = getAllAvailableAppsQuery;
        }

        public GetAllAvailableAppsQuery GetQuery()
        {
            return _getAllAvailableAppsQuery;
        }
    }
}
