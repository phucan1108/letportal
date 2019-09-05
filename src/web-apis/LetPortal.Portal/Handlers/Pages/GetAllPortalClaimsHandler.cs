using LetPortal.Portal.Handlers.Pages.Requests;
using LetPortal.Portal.Models.Pages;
using LetPortal.Portal.Repositories.Pages;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.Pages
{
    public class GetAllPortalClaimsHandler : IRequestHandler<GetAllPortalClaimsRequest, List<ShortPortalClaimModel>>
    {
        private readonly IPageRepository _pageRepository;

        public GetAllPortalClaimsHandler(IPageRepository pageRepository)
        {
            _pageRepository = pageRepository;
        }

        public async Task<List<ShortPortalClaimModel>> Handle(GetAllPortalClaimsRequest request, CancellationToken cancellationToken)
        {
            return await _pageRepository.GetShortPortalClaimModels();
        }
    }
}
