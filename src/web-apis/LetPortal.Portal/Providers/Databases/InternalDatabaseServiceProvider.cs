using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Models;
using LetPortal.Portal.Models.Databases;
using LetPortal.Portal.Repositories.Databases;
using LetPortal.Portal.Services.Databases;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetPortal.Portal.Providers.Databases
{
    public class InternalDatabaseServiceProvider : IDatabaseServiceProvider
    {
        private readonly IDatabaseService _databaseService;

        private readonly IDatabaseRepository _databaseRepository;

        public InternalDatabaseServiceProvider(
            IDatabaseService databaseService,
            IDatabaseRepository databaseRepository)
        {
            _databaseService = databaseService;
            _databaseRepository = databaseRepository;
        }

        public async Task<ExecuteDynamicResultModel> ExecuteDatabase(string databaseId, string formattedCommand, IEnumerable<ExecuteParamModel> parameters)
        {
            var databaseConnection = await _databaseRepository.GetOneAsync(databaseId);            
            return await _databaseService.ExecuteDynamic(databaseConnection, formattedCommand, parameters);
        }

        public async Task<DatabaseConnection> GetOneDatabaseConnectionAsync(string databaseId)
        {
            return await _databaseRepository.GetOneAsync(databaseId);
        }

        public async Task<ExtractingSchemaQueryModel> GetSchemasByQuery(string databaseId, string queryJsonString, IEnumerable<ExecuteParamModel> parameters)
        {
            var databaseConnection = await _databaseRepository.GetOneAsync(databaseId);
            return await _databaseService.ExtractColumnSchema(databaseConnection, queryJsonString, parameters);
        }
    }
}
