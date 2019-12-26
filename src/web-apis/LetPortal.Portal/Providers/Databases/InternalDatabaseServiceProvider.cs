using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Models;
using LetPortal.Portal.Models.Databases;
using LetPortal.Portal.Repositories.Databases;
using LetPortal.Portal.Services.Databases;
using System.Collections.Generic;
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

        public async Task<IEnumerable<ComparisonResult>> CompareDatabases(IEnumerable<DatabaseConnection> databaseConnections)
        {
            var results = new List<ComparisonResult>();
            foreach(var databaseConnection in databaseConnections)
            {
                results.Add(await _databaseRepository.Compare(databaseConnection));
            }
            return results;
        }

        public async Task<ExecuteDynamicResultModel> ExecuteDatabase(string databaseId, string formattedCommand, IEnumerable<ExecuteParamModel> parameters)
        {
            var databaseConnection = await _databaseRepository.GetOneAsync(databaseId);
            return await _databaseService.ExecuteDynamic(databaseConnection, formattedCommand, parameters);
        }

        public async Task<IEnumerable<DatabaseConnection>> GetDatabaseConnectionsByIds(IEnumerable<string> ids)
        {
            return await _databaseRepository.GetAllByIdsAsync(ids);
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
