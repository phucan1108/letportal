using LetPortal.Portal.Entities.Apps;
using LetPortal.Portal.Handlers.Apps.Queries;
using MediatR;

namespace LetPortal.Portal.Handlers.Apps.Requests
{
    public class GetAppRequest : IRequest<App>
    {
        private readonly GetAppQuery _getAppQuery;

        public GetAppRequest(GetAppQuery getAppQuery)
        {
            _getAppQuery = getAppQuery;
        }

        public GetAppQuery GetQuery()
        {
            return _getAppQuery;
        }
    }
}
