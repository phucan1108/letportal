using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Handlers.Databases.Requests;
using LetPortal.Portal.Repositories.Databases;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.Databases
{
    public class GetOneDatabaseHandler : IRequestHandler<GetOneDatabaseRequest, DatabaseConnection>
    {
        private readonly IDatabaseRepository _databaseRepository;

        public GetOneDatabaseHandler(IDatabaseRepository databaseRepository)
        {
            _databaseRepository = databaseRepository;
        }

        public async Task<DatabaseConnection> Handle(GetOneDatabaseRequest request, CancellationToken cancellationToken)
        {
            var result = await _databaseRepository.GetOneAsync(request.GetQuery().Id);

            return result;
        }
    }
}
