using System.Threading.Tasks;
using LetPortal.Portal.Entities.Components;
using LetPortal.Portal.Models.Charts;

namespace LetPortal.Portal.Services.Components
{
    public interface IChartService
    {
        Task<ExtractionChartFilter> Extract(ExtractingChartQueryModel model);

        Task<ExecutionChartResponseModel> Execute(Chart chart, ExecutionChartRequestModel model);
    }
}
