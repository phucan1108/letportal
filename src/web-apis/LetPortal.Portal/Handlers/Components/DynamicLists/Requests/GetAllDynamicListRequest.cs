using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Handlers.Components.DynamicLists.Queries;
using MediatR;
using System.Collections.Generic;

namespace LetPortal.Portal.Handlers.Components.DynamicLists.Requests
{
    public class GetAllDynamicListRequest : IRequest<List<DynamicList>>
    {
        private readonly GetAllDynamicListQuery _getAllDynamicListQuery;

        public GetAllDynamicListRequest(GetAllDynamicListQuery getAllDynamicListQuery)
        {
            _getAllDynamicListQuery = getAllDynamicListQuery;
        }

        public GetAllDynamicListQuery GetQuery()
        {
            return _getAllDynamicListQuery;
        }
    }
}
