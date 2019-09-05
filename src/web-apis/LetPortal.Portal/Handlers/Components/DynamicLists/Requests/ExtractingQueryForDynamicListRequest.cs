using LetPortal.Portal.Handlers.Components.DynamicLists.Queries;
using LetPortal.Portal.Models.DynamicLists;
using MediatR;

namespace LetPortal.Portal.Handlers.Components.DynamicLists.Requests
{
    public class ExtractingQueryForDynamicListRequest : IRequest<PopulateQueryModel>
    {
        private readonly ExtractingQueryForDynamicListQuery _extractingForDynamicListQuery;

        public ExtractingQueryForDynamicListRequest(ExtractingQueryForDynamicListQuery extractingForDynamicListQuery)
        {
            _extractingForDynamicListQuery = extractingForDynamicListQuery;
        }

        public ExtractingQueryForDynamicListQuery GetQuery()
        {
            return _extractingForDynamicListQuery;
        }
    }
}
