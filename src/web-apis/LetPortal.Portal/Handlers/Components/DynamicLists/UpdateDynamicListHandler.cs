using LetPortal.Portal.Handlers.Components.DynamicLists.Requests;
using LetPortal.Portal.Repositories.Components;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.Components.DynamicLists
{
    public class UpdateDynamicListHandler : IRequestHandler<UpdateDynamicLIstRequest>
    {
        private readonly IDynamicListRepository _dynamicListSectionPartRepository;

        public UpdateDynamicListHandler(IDynamicListRepository dynamicListSectionPartRepository)
        {
            _dynamicListSectionPartRepository = dynamicListSectionPartRepository;
        }

        public async Task<Unit> Handle(UpdateDynamicLIstRequest request, CancellationToken cancellationToken)
        {
            var updatingDynamicList = request.GetCommand().DynamicList;
            var dynamicListId = request.GetCommand().DynamicListId;
            updatingDynamicList.Id = dynamicListId;
            await _dynamicListSectionPartRepository.UpdateAsync(dynamicListId, updatingDynamicList);
            return Unit.Value;
        }
    }
}
