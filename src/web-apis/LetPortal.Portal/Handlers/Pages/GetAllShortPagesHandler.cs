using LetPortal.Portal.Handlers.Pages.Queries;
using LetPortal.Portal.Handlers.Pages.Requests;
using LetPortal.Portal.Models.Pages;
using LetPortal.Portal.Repositories.Pages;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.Pages
{
    public class GetAllShortPagesHandler : IRequestHandler<GetAllShortPagesRequest, List<ShortPageModel>>
    {
        private readonly IPageRepository _pageRepository;

        public GetAllShortPagesHandler(IPageRepository pageRepository)
        {
            _pageRepository = pageRepository;
        }
        public async Task<List<ShortPageModel>> Handle(GetAllShortPagesRequest request, CancellationToken cancellationToken)
        {
            return await _pageRepository.GetAllShortPages();
        }
    }
}
