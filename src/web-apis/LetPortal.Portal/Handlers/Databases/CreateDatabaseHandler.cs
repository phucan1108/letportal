using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Handlers.Databases.Requests;
using LetPortal.Portal.Repositories.Databases;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.Databases
{
    public class CreateDatabaseHandler : IRequestHandler<CreateDatabaseRequest, DatabaseConnection>
    {
        private readonly IDatabaseRepository _databaseRepository;

        public CreateDatabaseHandler(IDatabaseRepository databaseRepository)
        {
            _databaseRepository = databaseRepository;
        }

        public async Task<DatabaseConnection> Handle(CreateDatabaseRequest request, CancellationToken cancellationToken)
        {
            DatabaseConnection database = new DatabaseConnection
            {
                Id = DataUtil.GenerateUniqueId(),
                Name = request.GetCommand().Name,
                ConnectionString = request.GetCommand().ConnectionString,
                DataSource = request.GetCommand().DataSource,
                DatabaseConnectionType = request.GetCommand().DatabaseConnectionType
            };

            await _databaseRepository.AddAsync(database);

            return database;
        }
    }
}
