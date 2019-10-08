using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Models;
using LetPortal.Portal.Models.Databases;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetPortal.Portal.Executions
{
    public interface IExecutionDatabase
    {
        ConnectionType ConnectionType { get; }

        Task<ExecuteDynamicResultModel> Execute(DatabaseConnection databaseConnection, string formattedString, IEnumerable<ExecuteParamModel> parameters);
    }
}
