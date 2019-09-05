using LetPortal.Portal.Handlers.EntitySchemas.Queries;
using MediatR;

namespace LetPortal.Portal.Handlers.EntitySchemas.Requests
{
    public class ExecuteEntityQueryRequest : IRequest<object>
    {
        private readonly ExecuteEntityQuery _executeEntityQuery;

        public ExecuteEntityQueryRequest(ExecuteEntityQuery executeEntityQuery)
        {
            _executeEntityQuery = executeEntityQuery;
        }

        public ExecuteEntityQuery GetQuery()
        {
            return _executeEntityQuery;
        }
    }
}
