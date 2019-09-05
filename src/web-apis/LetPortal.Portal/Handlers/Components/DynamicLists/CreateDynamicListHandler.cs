using LetPortal.Core.Utils;
using LetPortal.Portal.Handlers.Components.DynamicLists.Requests;
using LetPortal.Portal.Repositories.Components;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.Components.DynamicLists
{
    public class CreateDynamicListHandler : IRequestHandler<CreateDynamicListRequest, string>
    {
        private readonly IDynamicListRepository _dynamicListSectionPartRepository;

        public CreateDynamicListHandler(IDynamicListRepository dynamicListSectionPartRepository)
        {
            _dynamicListSectionPartRepository = dynamicListSectionPartRepository;
        }

        public async Task<string> Handle(CreateDynamicListRequest request, CancellationToken cancellationToken)
        {
            var dynamicList = request.GetCommand().DynamicList;
            dynamicList.Id = DataUtil.GenerateUniqueId();
            await _dynamicListSectionPartRepository.AddAsync(dynamicList);
            return dynamicList.Id;
        }
    }
}
