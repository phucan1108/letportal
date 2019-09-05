using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Handlers.Databases.Queries;
using LetPortal.Portal.Handlers.Databases.Requests;
using LetPortal.Portal.Repositories.Databases;
using MediatR;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using LetPortal.Portal.Models.Databases;
using LetPortal.Portal.Models;
using LetPortal.Portal.Handlers.Databases.Commands;

namespace LetPortal.Portal.Providers.Databases
{
    public class InternalDatabaseServiceProvider : IDatabaseServiceProvider
    {
        private readonly IMediator _mediator;

        private readonly IDatabaseRepository _databaseRepository;

        public InternalDatabaseServiceProvider(IMediator mediator, IDatabaseRepository databaseRepository)
        {
            _mediator = mediator;
            _databaseRepository = databaseRepository;
        }

        public async Task<ExecuteDynamicResultModel> ExecuteDatabase(string databaseId, string formattedCommand)
        {            
            return await _mediator.Send(new ExecuteDynamicRequest(new ExecuteDynamicCommand { DatabaseId = databaseId, FormattedCommand = formattedCommand }));
        }

        public async Task<DatabaseConnection> GetOneDatabaseConnectionAsync(string databaseId)
        {
            return await _mediator.Send(new GetOneDatabaseRequest(new GetOneDatabaseQuery { Id = databaseId }));
        }

        public async Task<ExtractingSchemaQueryModel> GetSchemasByQuery(string databaseId, string queryJsonString)
        {
            return await _mediator.Send(new ExtractingColumnSchemaRequest(new ExtractingColumnSchemaQuery { DatabaseId = databaseId, QueryJsonString = queryJsonString }));
        }
    }
}
