using LetPortal.Portal.Handlers.Apps.Commands;
using MediatR;

namespace LetPortal.Portal.Handlers.Apps.Requests
{
    public class UpdateAppMenuRequest : IRequest
    {
        private readonly UpdateAppMenuCommand _updateAppMenuCommand;

        public UpdateAppMenuRequest(UpdateAppMenuCommand updateAppMenuCommand)
        {
            _updateAppMenuCommand = updateAppMenuCommand;
        }

        public UpdateAppMenuCommand GetCommand()
        {
            return _updateAppMenuCommand;
        }
    }
}
