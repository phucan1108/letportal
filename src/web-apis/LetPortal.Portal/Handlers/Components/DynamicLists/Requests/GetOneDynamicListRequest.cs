using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Handlers.Components.DynamicLists.Queries;
using MediatR;

namespace LetPortal.Portal.Handlers.Components.DynamicLists.Requests
{
    public class GetOneDynamicListRequest : IRequest<DynamicList>
    {
        private readonly GetOneDynamicListQuery _getOneDynamicListQuery;

        public GetOneDynamicListRequest(GetOneDynamicListQuery getOneDynamicListQuery)
        {
            _getOneDynamicListQuery = getOneDynamicListQuery;
        }

        public GetOneDynamicListQuery GetQuery()
        {
            return _getOneDynamicListQuery;
        }
    }
}
