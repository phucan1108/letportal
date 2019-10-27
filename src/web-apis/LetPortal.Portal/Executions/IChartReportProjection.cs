using System.Data;
using System.Threading.Tasks;

namespace LetPortal.Portal.Executions
{
    public interface IChartReportProjection
    {
        Task<dynamic> ProjectionFromDataTable(DataTable dataTable, string mappringProjection);
    }
}
