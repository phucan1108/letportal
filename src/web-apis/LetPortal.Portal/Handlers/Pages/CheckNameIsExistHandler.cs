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
    public class CheckNameIsExistHandler : IRequestHandler<CheckNameIsExistRequest, bool>
    {
        private readonly IPageRepository _pageRepository;

        public CheckNameIsExistHandler(IPageRepository pageRepository)
        {
            _pageRepository = pageRepository;
        }

        public async Task<bool> Handle(CheckNameIsExistRequest request, CancellationToken cancellationToken)
        {
            return await _pageRepository.IsExistAsync(request.GetQuery().Name);
        }
    }
}
