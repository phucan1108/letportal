using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Handlers.Databases.Commands;
using MediatR;

namespace LetPortal.Portal.Handlers.Databases.Requests
{
    public class CreateDatabaseRequest : IRequest<DatabaseConnection>
    {
        private readonly CreateDatabaseCommand _createDatabaseCommand;

        public CreateDatabaseRequest(CreateDatabaseCommand createDatabaseCommand)
        {
            _createDatabaseCommand = createDatabaseCommand;
        }

        public CreateDatabaseCommand GetCommand()
        {
            return _createDatabaseCommand;
        }
    }
}
