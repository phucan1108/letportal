using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Exceptions.Databases;
using LetPortal.Portal.Executions;
using LetPortal.Portal.Models;
using LetPortal.Portal.Models.Databases;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<ExecuteDynamicResultModel> ExecuteDynamic(DatabaseConnection databaseConnection, string formattedString)
        {
            var connectionType = databaseConnection.GetConnectionType();
            var executionDatabase = _executionDatabases.FirstOrDefault(a => a.ConnectionType == connectionType);

            if(executionDatabase != null)
            {
                switch(connectionType)
                {
                    case ConnectionType.MongoDB:
                        return await executionDatabase.Execute(GetMongoConnection(databaseConnection), formattedString);
                }
            }             
            throw new DatabaseException(DatabaseErrorCodes.NotSupportedConnectionType);
        }

        public async Task<ExtractingSchemaQueryModel> ExtractColumnSchema(DatabaseConnection databaseConnection, string formattedString)
        {
            var connectionType = databaseConnection.GetConnectionType();
            var extractionDatabase = _extractionDatabases.First(a => a.ConnectionType == connectionType);

            switch(connectionType)
            {
                case ConnectionType.MongoDB:
                    return await extractionDatabase.Extract(GetMongoConnection(databaseConnection), formattedString);
            }
            throw new DatabaseException(DatabaseErrorCodes.NotSupportedConnectionType);
        }

        private DefaultMongoConnection GetMongoConnection(DatabaseConnection databaseConnection)
        {
            return new DefaultMongoConnection { ConnectionString = databaseConnection.ConnectionString, DatabaseName = databaseConnection.DataSource };
        }
    }
}
