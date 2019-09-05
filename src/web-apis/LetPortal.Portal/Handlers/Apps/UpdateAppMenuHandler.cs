using LetPortal.Portal.Handlers.Apps.Requests;
using LetPortal.Portal.Repositories.Apps;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.Apps
{
    public class UpdateAppMenuHandler : AsyncRequestHandler<UpdateAppMenuRequest>
    {
        private readonly IAppRepository _appRepository;

        public UpdateAppMenuHandler(IAppRepository appRepository)
        {
            _appRepository = appRepository;
        }


        protected override async Task Handle(UpdateAppMenuRequest request, CancellationToken cancellationToken)
        {
            await _appRepository.UpdateMenuAsync(request.GetCommand().AppId, request.GetCommand().Menus);
        }
    }
}
