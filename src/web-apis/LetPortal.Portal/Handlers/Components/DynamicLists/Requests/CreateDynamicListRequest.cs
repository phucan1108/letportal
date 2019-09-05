using LetPortal.Portal.Handlers.Components.DynamicLists.Commands;
using MediatR;

namespace LetPortal.Portal.Handlers.Components.DynamicLists.Requests
{
    public class CreateDynamicListRequest : IRequest<string>
    {
        private readonly CreateDynamicListCommand _createDynamicListCommand;

        public CreateDynamicListRequest(CreateDynamicListCommand createDynamicListCommand)
        {
            _createDynamicListCommand = createDynamicListCommand;
        }

        public CreateDynamicListCommand GetCommand()
        {
            return _createDynamicListCommand;
        }
    }
}
