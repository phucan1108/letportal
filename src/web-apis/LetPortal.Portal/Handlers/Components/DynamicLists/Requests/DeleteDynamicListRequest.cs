using LetPortal.Portal.Handlers.Components.DynamicLists.Commands;
using MediatR;

namespace LetPortal.Portal.Handlers.Components.DynamicLists.Requests
{
    public class DeleteDynamicListRequest : IRequest
    {
        private readonly DeleteDynamicListCommand _deleteDynamicListCommand;

        public DeleteDynamicListRequest(DeleteDynamicListCommand deleteDynamicListCommand)
        {
            _deleteDynamicListCommand = deleteDynamicListCommand;
        }

        public DeleteDynamicListCommand GetCommand()
        {
            return _deleteDynamicListCommand;
        }
    }
}
