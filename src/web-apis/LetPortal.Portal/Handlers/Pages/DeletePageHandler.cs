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
    public class DeletePageHandler : IRequestHandler<DeletePageRequest>
    {
        private readonly IPageRepository _pageRepository;

        public DeletePageHandler(IPageRepository pageRepository)
        {
            _pageRepository = pageRepository;
        }

        public async Task<Unit> Handle(DeletePageRequest request, CancellationToken cancellationToken)
        {
            await _pageRepository.DeleteAsync(request.GetCommand().PageId);
            return Unit.Value;
        }
    }
}
