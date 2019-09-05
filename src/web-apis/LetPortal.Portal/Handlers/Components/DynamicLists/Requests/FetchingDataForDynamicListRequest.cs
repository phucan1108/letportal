using LetPortal.Portal.Handlers.Components.DynamicLists.Queries;
using LetPortal.Portal.Models.DynamicLists;
using MediatR;

namespace LetPortal.Portal.Handlers.Components.DynamicLists.Requests
{
    public class FetchingDataForDynamicListRequest : IRequest<DynamicListResponseDataModel>
    {
        private readonly FetchingDataForDynamicListQuery _fetchingDataForDynamicListQuery;

        public FetchingDataForDynamicListRequest(FetchingDataForDynamicListQuery fetchingDataForDynamicListQuery)
        {
            _fetchingDataForDynamicListQuery = fetchingDataForDynamicListQuery;
        }

        public FetchingDataForDynamicListQuery GetQuery()
        {
            return _fetchingDataForDynamicListQuery;
        }
    }
}
