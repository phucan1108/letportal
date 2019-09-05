using LetPortal.Portal.Handlers.Databases.Commands;
using MediatR;

namespace LetPortal.Portal.Handlers.Databases.Requests
{
    public class DeleteDatabaseRequest : IRequest
    {
        private readonly DeleteDatabaseCommand _deleteDatabaseCommand;

        public DeleteDatabaseRequest(DeleteDatabaseCommand deleteDatabaseCommand)
        {
            _deleteDatabaseCommand = deleteDatabaseCommand;
        }

        public DeleteDatabaseCommand GetCommand()
        {
            return _deleteDatabaseCommand;
        }
    }
}
