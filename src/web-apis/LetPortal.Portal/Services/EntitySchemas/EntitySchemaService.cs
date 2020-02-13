using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Portal.Entities.EntitySchemas;
using LetPortal.Portal.Executions;
using LetPortal.Portal.Providers.Databases;
using MongoDB.Driver;

namespace LetPortal.Portal.Services.EntitySchemas
{
    public class EntitySchemaService : IEntitySchemaService
    {
        private readonly IDatabaseServiceProvider _databaseServiceProvider;

        private readonly IEnumerable<IAnalyzeDatabase> _analyzeDatabases;

        public EntitySchemaService(
            IDatabaseServiceProvider databaseServiceProvider,
            IEnumerable<IAnalyzeDatabase> analyzeDatabases)
        {
            _databaseServiceProvider = databaseServiceProvider;
            _analyzeDatabases = analyzeDatabases;
        }

        public async Task<IEnumerable<EntitySchema>> FetchAllEntitiesFromDatabase(string databaseId)
        {
            var database = await _databaseServiceProvider.GetOneDatabaseConnectionAsync(databaseId);

            var foundAnalyzeDatabase = _analyzeDatabases.First(a => a.ConnectionType == database.GetConnectionType());

            return await foundAnalyzeDatabase.FetchAllEntitiesFromDatabase(database);
        }
    }
}
