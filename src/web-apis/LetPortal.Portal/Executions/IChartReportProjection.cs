using System.Data;
using System.Threading.Tasks;

namespace LetPortal.Portal.Executions
{
    public interface IChartReportProjection
    {
        Task<object> ProjectionFromDataTable(DataTable dataTable, string mappringProjection);
    }
}
