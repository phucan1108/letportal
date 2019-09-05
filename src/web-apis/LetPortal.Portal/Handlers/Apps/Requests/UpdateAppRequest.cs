using LetPortal.Portal.Handlers.Apps.Commands;
using MediatR;

namespace LetPortal.Portal.Handlers.Apps.Requests
{
    public class UpdateAppRequest : IRequest
    {
        private readonly UpdateAppCommand _updateAppCommand;

        public UpdateAppRequest(UpdateAppCommand updateAppCommand)
        {
            _updateAppCommand = updateAppCommand;
        }

        public UpdateAppCommand GetCommand()
        {
            return _updateAppCommand;
        }
    }
}
