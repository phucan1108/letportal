using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Models;
using LetPortal.Portal.Models.Databases;

namespace LetPortal.Portal.Providers.Databases
{
    public interface IDatabaseServiceProvider
    {
        Task<IEnumerable<DatabaseConnection>> GetDatabaseConnectionsByIds(IEnumerable<string> ids);

        Task<IEnumerable<ComparisonResult>> CompareDatabases(IEnumerable<DatabaseConnection> databaseConnections);

        Task<DatabaseConnection> GetOneDatabaseConnectionAsync(string databaseId);

        Task<ExtractingSchemaQueryModel> GetSchemasByQuery(
            string databaseId,
            string queryJsonString,
            IEnumerable<ExecuteParamModel> parameters);

        Task<ExecuteDynamicResultModel> ExecuteDatabase(string databaseId, string formattedCommand, IEnumerable<ExecuteParamModel> parameters);

        Task ForceUpdateDatabases(IEnumerable<DatabaseConnection> databases);
    }
}
