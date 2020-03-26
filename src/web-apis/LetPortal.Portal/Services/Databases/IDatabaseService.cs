using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Entities.Shared;
using LetPortal.Portal.Models;
using LetPortal.Portal.Models.Databases;
using LetPortal.Portal.Models.Pages;

namespace LetPortal.Portal.Services.Databases
{
    public interface IDatabaseService
    {
        Task<ExecuteDynamicResultModel> ExecuteDynamic(
            DatabaseConnection databaseConnection, 
            string formattedString, 
            IEnumerable<ExecuteParamModel> parameters);

        Task<ExecuteDynamicResultModel> ExecuteDynamic(
            List<DatabaseConnection> databaseConnections,
            DatabaseExecutionChains executionChains,
            IEnumerable<ExecuteParamModel> parameters,
            IEnumerable<LoopDataParamModel> LoopDatas = null);

        Task<ExtractingSchemaQueryModel> ExtractColumnSchema(
            DatabaseConnection databaseConnection,
            string formattedString,
            IEnumerable<ExecuteParamModel> parameters);
    }
}
