using LetPortal.Portal.Entities.Apps;
using LetPortal.Portal.Handlers.Apps.Requests;
using LetPortal.Portal.Repositories.Apps;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.Apps
{
    public class GetAppHandler : IRequestHandler<GetAppRequest, App>
    {
        private readonly IAppRepository _appRepository;

        public GetAppHandler(IAppRepository appRepository)
        {
            _appRepository = appRepository;
        }

        public async Task<App> Handle(GetAppRequest request, CancellationToken cancellationToken)
        {
            return await _appRepository.GetOneAsync(request.GetQuery().AppId);
        }
    }
}
