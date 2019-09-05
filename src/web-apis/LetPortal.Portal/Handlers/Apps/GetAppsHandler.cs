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
    public class GetAppsHandler : IRequestHandler<GetAppsRequest, List<App>>
    {
        private readonly IAppRepository _appRepository;

        public GetAppsHandler(IAppRepository appRepository)
        {
            _appRepository = appRepository;
        }

        public Task<List<App>> Handle(GetAppsRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_appRepository.GetAsQueryable().Where(a => request.GetQuery().AppIds.Contains(a.Id)).ToList());
        }
    }
}
