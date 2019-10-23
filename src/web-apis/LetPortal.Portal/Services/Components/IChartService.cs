using LetPortal.Portal.Entities.Components;
using LetPortal.Portal.Models.Charts;
using System.Threading.Tasks;

namespace LetPortal.Portal.Services.Components
{
    public interface IChartService
    {
        Task<ExtractionChartFilter> Extract(ExtractingChartQueryModel model);

        Task<ExecutionChartResponseModel> Execute(Chart chart,ExecutionChartRequestModel model);
    }
}
