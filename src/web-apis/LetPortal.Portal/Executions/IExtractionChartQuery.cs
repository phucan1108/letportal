using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Models.Charts;

namespace LetPortal.Portal.Executions
{
    public interface IExtractionChartQuery
    {
        ConnectionType ConnectionType { get; }

        Task<ExtractionChartFilter> Extract(
            DatabaseConnection databaseConnection,
            string formattedString,
            IEnumerable<ChartParameterValue> parameterValues);
    }
}
