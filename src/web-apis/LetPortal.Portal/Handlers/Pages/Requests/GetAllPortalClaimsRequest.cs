using LetPortal.Portal.Handlers.Pages.Queries;
using LetPortal.Portal.Models.Pages;
using MediatR;
using System.Collections.Generic;

namespace LetPortal.Portal.Handlers.Pages.Requests
{
    public class GetAllPortalClaimsRequest : IRequest<List<ShortPortalClaimModel>>
    {
        private readonly GetAllPortalClaimsQuery _getAllPortalClaimsQuery;

        public GetAllPortalClaimsRequest(GetAllPortalClaimsQuery getAllPortalClaimsQuery)
        {
            _getAllPortalClaimsQuery = getAllPortalClaimsQuery;
        }

        public GetAllPortalClaimsQuery GetQuery()
        {
            return _getAllPortalClaimsQuery;
        }
    }
}
