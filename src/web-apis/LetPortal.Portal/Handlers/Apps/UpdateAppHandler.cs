using LetPortal.Portal.Handlers.Apps.Requests;
using LetPortal.Portal.Repositories.Apps;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.Apps
{
    public class UpdateAppHandler : AsyncRequestHandler<UpdateAppRequest>
    {
        private readonly IAppRepository _appRepository;

        public UpdateAppHandler(IAppRepository appRepository)
        {
            _appRepository = appRepository;
        }

        protected override async Task Handle(UpdateAppRequest request, CancellationToken cancellationToken)
        {
            await _appRepository.UpdateAsync(request.GetCommand().AppId, request.GetCommand().App);
        }
    }
}
