using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Entities.Shared;
using LetPortal.Portal.Models;
using LetPortal.Portal.Models.Databases;
using LetPortal.Portal.Models.Pages;
using LetPortal.Portal.Repositories.Databases;
using LetPortal.Portal.Services.Databases;

namespace LetPortal.Portal.Providers.Databases
{
    public class InternalDatabaseServiceProvider : IDatabaseServiceProvider, IDisposable
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
            foreach (var databaseConnection in databaseConnections)
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

        public async Task ForceUpdateDatabases(IEnumerable<DatabaseConnection> databases)
        {
            foreach (var database in databases)
            {
                await _databaseRepository.ForceUpdateAsync(database.Id, database);
            }
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

        public async Task<ExecuteDynamicResultModel> ExecuteDatabase(
            DatabaseExecutionChains databaseExecutionChains, 
            IEnumerable<ExecuteParamModel> parameters,
            IEnumerable<LoopDataParamModel> LoopDatas = null)
        {
            var allRequiredDb = await _databaseRepository.GetAllByIdsAsync(databaseExecutionChains?.Steps.Select(a => a.DatabaseConnectionId));

            return await _databaseService
                        .ExecuteDynamic(
                            allRequiredDb?.ToList(), 
                            databaseExecutionChains, 
                            parameters,
                            LoopDatas);
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _databaseRepository.Dispose();
                }

                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
