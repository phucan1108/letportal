using LetPortal.Portal.Entities.Pages;
using LetPortal.Portal.Handlers.Pages.Requests;
using LetPortal.Portal.Repositories.Pages;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.Pages
{
    public class GetOnePageHandler : IRequestHandler<GetOnePageRequest, Page>
    {
        private readonly IPageRepository _pageRepository;
        public GetOnePageHandler(IPageRepository pageRepository)
        {
            _pageRepository = pageRepository;
        } 
        public async Task<Page> Handle(GetOnePageRequest request, CancellationToken cancellationToken)
        {
            return await _pageRepository.GetOneByName(request.GetQuery().PageName);
        }
    }
}
