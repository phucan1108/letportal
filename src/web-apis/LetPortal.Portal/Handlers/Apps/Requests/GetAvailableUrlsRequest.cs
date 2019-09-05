using LetPortal.Portal.Handlers.Apps.Queries;
using LetPortal.Portal.Models.Apps;
using MediatR;
using System.Collections.Generic;

namespace LetPortal.Portal.Handlers.Apps.Requests
{
    public class GetAvailableUrlsRequest : IRequest<List<AvailableUrl>>
    {
        private readonly GetAvailableUrlsQuery _getAvailableUrlsQuery;

        public GetAvailableUrlsRequest(GetAvailableUrlsQuery getAvailableUrlsQuery)
        {
            _getAvailableUrlsQuery = getAvailableUrlsQuery;
        }

        public GetAvailableUrlsQuery GetQuery()
        {
            return _getAvailableUrlsQuery;
        }
    }
}
