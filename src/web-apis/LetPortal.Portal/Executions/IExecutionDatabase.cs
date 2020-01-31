using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Models;
using LetPortal.Portal.Models.Databases;

namespace LetPortal.Portal.Executions
{
    public interface IExecutionDatabase
    {
        ConnectionType ConnectionType { get; }

        Task<ExecuteDynamicResultModel> Execute(DatabaseConnection databaseConnection, string formattedString, IEnumerable<ExecuteParamModel> parameters);
    }
}
