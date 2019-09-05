using LetPortal.Portal.Entities.Apps;
using LetPortal.Portal.Handlers.Apps.Requests;
using LetPortal.Portal.Repositories.Apps;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.Apps
{
    public class GetAllAvailableAppsHandler : IRequestHandler<GetAllAvailableAppsRequest, List<App>>
    {
        private readonly IAppRepository _appRepository;

        public GetAllAvailableAppsHandler(IAppRepository appRepository)
        {
            _appRepository = appRepository;
        }

        public Task<List<App>> Handle(GetAllAvailableAppsRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_appRepository.GetAsQueryable().ToList());
        }
    }
}
