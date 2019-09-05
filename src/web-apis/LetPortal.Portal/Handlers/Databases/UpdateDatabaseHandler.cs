using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Handlers.Databases.Requests;
using LetPortal.Portal.Repositories.Databases;
using MediatR;

namespace LetPortal.Portal.Handlers.Databases
{
    public class UpdateDatabaseHandler : RequestHandler<UpdateDatabaseRequest>
    {
        private readonly IDatabaseRepository _databaseRepository;

        public UpdateDatabaseHandler(IDatabaseRepository databaseRepository)
        {
            _databaseRepository = databaseRepository;
        }

        protected override void Handle(UpdateDatabaseRequest request)
        {
            DatabaseConnection updatedDatabase = new DatabaseConnection
            {
                Id = request.GetCommand().Id,
                Name = request.GetCommand().Name,
                DataSource = request.GetCommand().DataSource,
                ConnectionString = request.GetCommand().ConnectionString,
                DatabaseConnectionType = request.GetCommand().DatabaseConnectionType
            };

            _databaseRepository.UpdateAsync(request.GetCommand().Id, updatedDatabase).Wait();
        }
    }
}
