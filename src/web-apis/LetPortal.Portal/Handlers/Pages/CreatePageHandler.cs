using LetPortal.Core.Utils;
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
    public class CreatePageHandler : IRequestHandler<CreatePageRequest, string>
    {
        private readonly IPageRepository _pageRepository;

        public CreatePageHandler(IPageRepository pageRepository)
        {
            _pageRepository = pageRepository;
        }

        public async Task<string> Handle(CreatePageRequest request, CancellationToken cancellationToken)
        {
            var page = request.GetCommand().Page;
            page.Id = DataUtil.GenerateUniqueId();
            await _pageRepository.AddAsync(page);

            return page.Id;
        }
    }
}
