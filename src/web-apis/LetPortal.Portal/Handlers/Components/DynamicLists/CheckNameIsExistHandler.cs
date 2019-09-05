using LetPortal.Portal.Handlers.Components.DynamicLists.Requests;
using LetPortal.Portal.Repositories.Components;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.Components.DynamicLists
{
    public class CheckNameIsExistHandler : IRequestHandler<CheckNameIsExistRequest, bool>
    {
        private readonly IDynamicListRepository _dynamicListRepository;

        public CheckNameIsExistHandler(IDynamicListRepository dynamicListRepository)
        {
            _dynamicListRepository = dynamicListRepository;
        }

        public async Task<bool> Handle(CheckNameIsExistRequest request, CancellationToken cancellationToken)
        {
            return await _dynamicListRepository.IsExistAsync(request.GetQuery().Name);
        }
    }
}
