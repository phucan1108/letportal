using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Models;
using LetPortal.Portal.Models.Databases;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetPortal.Portal.Services.Databases
{
    public interface IDatabaseService
    {
        Task<ExecuteDynamicResultModel> ExecuteDynamic(DatabaseConnection databaseConnection, string formattedString, IEnumerable<ExecuteParamModel> parameters);

        Task<ExtractingSchemaQueryModel> ExtractColumnSchema(
            DatabaseConnection databaseConnection,
            string formattedString,
            IEnumerable<ExecuteParamModel> parameters);
    }
}
