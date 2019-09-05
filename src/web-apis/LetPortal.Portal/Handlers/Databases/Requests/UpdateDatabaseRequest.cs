using LetPortal.Portal.Handlers.Databases.Commands;
using MediatR;

namespace LetPortal.Portal.Handlers.Databases.Requests
{
    public class UpdateDatabaseRequest : IRequest
    {
        private readonly UpdateDatabaseCommand _updateDatabaseCommand;

        public UpdateDatabaseRequest(UpdateDatabaseCommand updateDatabaseCommand)
        {
            _updateDatabaseCommand = updateDatabaseCommand;
        }

        public UpdateDatabaseCommand GetCommand()
        {
            return _updateDatabaseCommand;
        }
    }
}
