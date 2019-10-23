using LetPortal.Portal.Entities.Components;
using LetPortal.Portal.Executions;
using LetPortal.Portal.Models.Charts;
using LetPortal.Portal.Providers.Databases;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetPortal.Portal.Services.Components
{
    public class ChartService : IChartService
    {
        private readonly IDatabaseServiceProvider _databaseServiceProvider;

        private readonly IEnumerable<IExtractionChartQuery> _extractionChartQueries;

        private readonly IEnumerable<IExecutionChartReport> _executionChartReports;

        public ChartService(
            IDatabaseServiceProvider databaseServiceProvider, 
            IEnumerable<IExtractionChartQuery> extractionChartQueries,
            IEnumerable<IExecutionChartReport> executionChartReports)
        {
            _databaseServiceProvider = databaseServiceProvider;
            _extractionChartQueries = extractionChartQueries;
            _executionChartReports = executionChartReports;
        }

        public async Task<ExecutionChartResponseModel> Execute(Chart chart,ExecutionChartRequestModel model)
        {
            var databaseConnection = await _databaseServiceProvider.GetOneDatabaseConnectionAsync(chart.DatabaseOptions.DatabaseConnectionId);
            var foundExecution = _executionChartReports.First(a => a.ConnectionType == databaseConnection.GetConnectionType());

            return await foundExecution.Execute(databaseConnection, chart.DatabaseOptions.Query, chart.Definitions.MappingProjection, model.ChartParameterValues, model.ChartFilterValues);
        }

        public async Task<ExtractionChartFilter> Extract(ExtractingChartQueryModel model)
        {
            var databaseConnection = await _databaseServiceProvider.GetOneDatabaseConnectionAsync(model.DatabaseId);
            var foundExtraction = _extractionChartQueries.First(a => a.ConnectionType == databaseConnection.GetConnectionType());

            return await foundExtraction.Extract(databaseConnection, model.Query, model.Parameters.Select(a => new ChartParameterValue { Name = a.Name, Value = a.Value }));
        }
    }
}
