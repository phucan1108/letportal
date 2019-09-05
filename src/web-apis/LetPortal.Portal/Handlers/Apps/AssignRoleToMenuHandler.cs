using LetPortal.Portal.Handlers.Apps.Requests;
using LetPortal.Portal.Repositories.Apps;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.Apps
{
    public class AssignRoleToMenuHandler : IRequestHandler<AssignRoleToMenuRequest>
    {
        private readonly IAppRepository _appRepository;

        public AssignRoleToMenuHandler(IAppRepository appRepository)
        {
            _appRepository = appRepository;
        }

        public async Task<Unit> Handle(AssignRoleToMenuRequest request, CancellationToken cancellationToken)
        {
            var app = await _appRepository.GetOneAsync(request.GetCommand().AppId);
            var requestMenuProfile = request.GetCommand().MenuProfile;
            if(app.MenuProfiles.Any(a => a.Role == requestMenuProfile.Role))
            {
                foreach(var menuProfile in app.MenuProfiles)
                {
                    if(menuProfile.Role == request.GetCommand().MenuProfile.Role)
                    {
                        menuProfile.MenuIds = requestMenuProfile.MenuIds;
                        break;
                    }
                }
            }
            else
            {
                app.MenuProfiles.Add(requestMenuProfile);
            }

            await _appRepository.UpdateAsync(app.Id, app);

            return Unit.Value;
        }
    }
}
