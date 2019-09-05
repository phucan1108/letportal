using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Handlers.Databases.Requests;
using LetPortal.Portal.Repositories.Databases;
using MediatR;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Portal.Handlers.Databases
{
    public class GetAllDatabasesHandler : IRequestHandler<GetAllDatabasesRequest, List<DatabaseConnection>>
    {
        private readonly IDatabaseRepository _databaseRepository;

        public GetAllDatabasesHandler(IDatabaseRepository databaseRepository)
        {
            _databaseRepository = databaseRepository;
        }

        public Task<List<DatabaseConnection>> Handle(GetAllDatabasesRequest request, CancellationToken cancellationToken)
        {
            List<DatabaseConnection> results = _databaseRepository.GetAsQueryable().ToList();

            return Task.FromResult(results);
        }
    }
}
