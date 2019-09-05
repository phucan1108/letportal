using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Handlers.Components.DynamicLists.Requests;
using LetPortal.Portal.Repositories.Components;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.Components.DynamicLists
{
    public class GetOneDynamicListHandler : IRequestHandler<GetOneDynamicListRequest, DynamicList>
    {
        private readonly IDynamicListRepository _dynamicListSectionPartRepository;

        public GetOneDynamicListHandler(IDynamicListRepository dynamicListSectionPartRepository)
        {
            _dynamicListSectionPartRepository = dynamicListSectionPartRepository;
        }

        public async Task<DynamicList> Handle(GetOneDynamicListRequest request, CancellationToken cancellationToken)
        {
            return await _dynamicListSectionPartRepository.GetOneAsync(request.GetQuery().DynamicListId);
        }
    }
}
