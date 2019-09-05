using LetPortal.Portal.Handlers.Apps.Commands;
using MediatR;

namespace LetPortal.Portal.Handlers.Apps.Requests
{
    public class AssignRoleToMenuRequest : IRequest
    {
        private readonly AssignRoleToMenuCommand _assignRoleToMenuCommand;

        public AssignRoleToMenuRequest(AssignRoleToMenuCommand assignRoleToMenuCommand)
        {
            _assignRoleToMenuCommand = assignRoleToMenuCommand;
        }

        public AssignRoleToMenuCommand GetCommand()
        {
            return _assignRoleToMenuCommand;
        }
    }
}
