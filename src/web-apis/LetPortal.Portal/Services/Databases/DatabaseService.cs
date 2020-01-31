using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Exceptions.Databases;
using LetPortal.Portal.Executions;
using LetPortal.Portal.Models;
using LetPortal.Portal.Models.Databases;
using MongoDB.Driver;

namespace LetPortal.Portal.Services.Databases
{
    public class DatabaseService : IDatabaseService
    {
        private readonly IEnumerable<IExecutionDatabase> _executionDatabases;

        private readonly IEnumerable<IExtractionDatabase> _extractionDatabases;

        public DatabaseService(
            IEnumerable<IExecutionDatabase> executionDatabases,
            IEnumerable<IExtractionDatabase> extractionDatabases
            )
        {
            _executionDatabases = executionDatabases;
            _extractionDatabases = extractionDatabases;
        }

        public async Task<ExecuteDynamicResultModel> ExecuteDynamic(DatabaseConnection databaseConnection, string formattedString, IEnumerable<ExecuteParamModel> parameters)
        {
            var connectionType = databaseConnection.GetConnectionType();
            var executionDatabase = _executionDatabases.FirstOrDefault(a => a.ConnectionType == connectionType);

            if (executionDatabase != null)
            {
                return await executionDatabase.Execute(databaseConnection, formattedString, parameters);
            }
            throw new DatabaseException(DatabaseErrorCodes.NotSupportedConnectionType);
        }

        public async Task<ExtractingSchemaQueryModel> ExtractColumnSchema(DatabaseConnection databaseConnection, string formattedString, IEnumerable<ExecuteParamModel> parameters)
        {
            var connectionType = databaseConnection.GetConnectionType();
            var extractionDatabase = _extractionDatabases.First(a => a.ConnectionType == connectionType);

            return await extractionDatabase.Extract(databaseConnection, formattedString, parameters);
        }
    }
}
