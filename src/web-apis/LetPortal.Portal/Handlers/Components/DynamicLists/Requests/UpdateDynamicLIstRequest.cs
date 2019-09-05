using LetPortal.Portal.Handlers.Components.DynamicLists.Commands;
using MediatR;

namespace LetPortal.Portal.Handlers.Components.DynamicLists.Requests
{
    public class UpdateDynamicLIstRequest : IRequest
    {
        private readonly UpdateDynamicListCommand _updateDynamicListCommand;

        public UpdateDynamicLIstRequest(UpdateDynamicListCommand updateDynamicListCommand)
        {
            _updateDynamicListCommand = updateDynamicListCommand;
        }

        public UpdateDynamicListCommand GetCommand()
        {
            return _updateDynamicListCommand;
        }
    }
}
