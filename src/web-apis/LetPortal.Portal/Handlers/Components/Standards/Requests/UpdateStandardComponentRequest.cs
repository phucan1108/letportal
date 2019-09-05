using LetPortal.Portal.Handlers.Components.Standards.Commands;
using MediatR;

namespace LetPortal.Portal.Handlers.Components.Standards.Requests
{
    public class UpdateStandardComponentRequest : IRequest
    {
        private readonly UpdateStandardComponentCommand _updateStandardComponentCommand;

        public UpdateStandardComponentRequest(UpdateStandardComponentCommand updateStandardComponentCommand)
        {
            _updateStandardComponentCommand = updateStandardComponentCommand;
        }

        public UpdateStandardComponentCommand GetCommand()
        {
            return _updateStandardComponentCommand;
        }
    }
}
