using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Entities.Shared;
using LetPortal.Portal.Models;
using LetPortal.Portal.Models.Databases;
using Newtonsoft.Json.Linq;

namespace LetPortal.Portal.Executions
{
    public interface IExecutionDatabase
    {
        ConnectionType ConnectionType { get; }

        Task<ExecuteDynamicResultModel> Execute(
            DatabaseConnection databaseConnection, 
            string formattedString, 
            IEnumerable<ExecuteParamModel> parameters);

        Task<StepExecutionResult> ExecuteStep(
            DatabaseConnection databaseConnection, 
            string formattedString, 
            IEnumerable<ExecuteParamModel> parameters, 
            ExecutionDynamicContext context);
    }

    public class ExecutionDynamicContext
    {
        public JObject Data { get; set; }
    }
}
