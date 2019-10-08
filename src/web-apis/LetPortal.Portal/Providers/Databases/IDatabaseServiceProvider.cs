using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Models;
using LetPortal.Portal.Models.Databases;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetPortal.Portal.Providers.Databases
{
    public interface IDatabaseServiceProvider
    {
        Task<DatabaseConnection> GetOneDatabaseConnectionAsync(string databaseId);

        Task<ExtractingSchemaQueryModel> GetSchemasByQuery(string databaseId, string queryJsonString);

        Task<ExecuteDynamicResultModel> ExecuteDatabase(string databaseId, string formattedCommand, IEnumerable<ExecuteParamModel> parameters);
    }
}
