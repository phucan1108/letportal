using LetPortal.Portal.Entities.Apps;
using LetPortal.Portal.Handlers.Apps.Queries;
using MediatR;
using System.Collections.Generic;

namespace LetPortal.Portal.Handlers.Apps.Requests
{
    public class GetAppsRequest : IRequest<List<App>>
    {
        private readonly GetAppsQuery _getAppsQuery;

        public GetAppsRequest(GetAppsQuery getAppsQuery)
        {
            _getAppsQuery = getAppsQuery;
        }

        public GetAppsQuery GetQuery()
        {
            return _getAppsQuery;
        }
    }
}
