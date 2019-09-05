using LetPortal.Portal.Entities.Apps;
using LetPortal.Portal.Handlers.Apps.Commands;
using MediatR;

namespace LetPortal.Portal.Handlers.Apps.Requests
{
    public class CreateAppRequest : IRequest<App>
    {
        private readonly CreateAppCommand _createAppCommand;

        public CreateAppRequest(CreateAppCommand createAppCommand)
        {
            _createAppCommand = createAppCommand;
        }

        public CreateAppCommand GetCommand()
        {
            return _createAppCommand;
        }
    }
}
