using LetPortal.Portal.Handlers.Databases.Commands;
using LetPortal.Portal.Models;
using MediatR;

namespace LetPortal.Portal.Handlers.Databases.Requests
{
    public class ExecuteDynamicRequest : IRequest<ExecuteDynamicResultModel>
    {
        private readonly ExecuteDynamicCommand _executeDynamicCommand;

        public ExecuteDynamicRequest(ExecuteDynamicCommand executeDynamicCommand)
        {
            _executeDynamicCommand = executeDynamicCommand;
        }

        public ExecuteDynamicCommand GetCommand()
        {
            return _executeDynamicCommand;
        }
    }
}
