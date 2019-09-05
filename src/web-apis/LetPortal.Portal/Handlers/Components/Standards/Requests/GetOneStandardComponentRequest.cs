using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Handlers.Components.Standards.Queries;
using MediatR;

namespace LetPortal.Portal.Handlers.Components.Standards.Requests
{
    public class GetOneStandardComponentRequest : IRequest<StandardComponent>
    {
        private readonly GetOneStandardComponentQuery _getOneStandardComponentQuery;

        public GetOneStandardComponentRequest(GetOneStandardComponentQuery getOneStandardComponentQuery)
        {
            _getOneStandardComponentQuery = getOneStandardComponentQuery;
        }

        public GetOneStandardComponentQuery GetQuery()
        {
            return _getOneStandardComponentQuery;
        }
    }
}
