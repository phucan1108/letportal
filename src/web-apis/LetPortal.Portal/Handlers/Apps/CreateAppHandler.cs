using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Apps;
using LetPortal.Portal.Handlers.Apps.Requests;
using LetPortal.Portal.Repositories.Apps;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.Apps
{
    public class CreateAppHandler : IRequestHandler<CreateAppRequest, App>
    {
        private readonly IAppRepository _appRepository;

        public CreateAppHandler(IAppRepository appRepository)
        {
            _appRepository = appRepository;
        }

        public async Task<App> Handle(CreateAppRequest request, CancellationToken cancellationToken)
        {
            var app = request.GetCommand().App;

            app.Id = DataUtil.GenerateUniqueId();

            await _appRepository.AddAsync(app);

            return app;
        }
    }
}
