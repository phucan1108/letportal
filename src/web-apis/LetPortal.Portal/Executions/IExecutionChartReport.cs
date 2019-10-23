using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Models.Charts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetPortal.Portal.Executions
{
    public interface IExecutionChartReport
    {
        ConnectionType ConnectionType { get; }

        Task<ExecutionChartResponseModel> Execute(
            DatabaseConnection databaseConnection, 
            string formattedString, 
            string mappingProjection,
            IEnumerable<ChartParameterValue> parameters, 
            IEnumerable<ChartFilterValue> filterValues);
    }
}
