using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Handlers.Components.Standards.Queries;
using MediatR;
using System.Collections.Generic;

namespace LetPortal.Portal.Handlers.Components.Standards.Requests
{
    public class GetAllStandardComponentsByIdsRequest : IRequest<IEnumerable<StandardComponent>>
    {
        private readonly GetAllStandardComponentsByIdsQuery _getAllStandardComponentsByIdsQuery;

        public GetAllStandardComponentsByIdsRequest(GetAllStandardComponentsByIdsQuery getAllStandardComponentsByIdsQuery)
        {
            _getAllStandardComponentsByIdsQuery = getAllStandardComponentsByIdsQuery;
        }

        public GetAllStandardComponentsByIdsQuery GetQuery()
        {
            return _getAllStandardComponentsByIdsQuery;
        }
    }
}
