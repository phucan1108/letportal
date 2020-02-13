using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Models.Charts;

namespace LetPortal.Portal.Executions
{
    public interface IExecutionChartReport
    {
        ConnectionType ConnectionType { get; }

        Task<ExecutionChartResponseModel> Execute(ExecutionChartReportModel model);
    }

    public class ExecutionChartReportModel
    {
        public DatabaseConnection DatabaseConnection { get; set; }

        public string FormattedString { get; set; }

        public string MappingProjection { get; set; }

        public IEnumerable<ChartParameterValue> Parameters { get; set; }

        public IEnumerable<ChartFilterValue> FilterValues { get; set; }

        public bool IsRealTime { get; set; }

        public string ComparedRealTimeField { get; set; }

        public DateTime? LastComparedDate { get; set; }
    }
}
