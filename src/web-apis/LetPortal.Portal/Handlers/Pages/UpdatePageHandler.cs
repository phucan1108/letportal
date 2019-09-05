using LetPortal.Portal.Handlers.Pages.Requests;
using LetPortal.Portal.Repositories.Pages;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.Pages
{
    public class UpdatePageHandler : IRequestHandler<UpdatePageRequest>
    {
        private readonly IPageRepository _pageRepository;

        public UpdatePageHandler(IPageRepository pageRepository)
        {
            _pageRepository = pageRepository;
        }

        public async Task<Unit> Handle(UpdatePageRequest request, CancellationToken cancellationToken)
        {
            await _pageRepository.UpdateAsync(request.GetCommand().PageId, request.GetCommand().Page);
            return Unit.Value;
        }
    }
}
