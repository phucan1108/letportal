using LetPortal.Portal.Handlers.Components.DynamicLists.Requests;
using LetPortal.Portal.Repositories.Components;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.Components.DynamicLists
{
    public class DeleteDynamicListHandler : IRequestHandler<DeleteDynamicListRequest>
    {
        private readonly IDynamicListRepository _dynamicListSectionPartRepository;

        public DeleteDynamicListHandler(IDynamicListRepository dynamicListSectionPartRepository)
        {
            _dynamicListSectionPartRepository = dynamicListSectionPartRepository;
        }

        public async Task<Unit> Handle(DeleteDynamicListRequest request, CancellationToken cancellationToken)
        {
            await _dynamicListSectionPartRepository.DeleteAsync(request.GetCommand().Id);

            return Unit.Value;
        }
    }
}
