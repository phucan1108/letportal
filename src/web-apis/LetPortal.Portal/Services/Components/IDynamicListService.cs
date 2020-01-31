using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Models.DynamicLists;
using System.Threading.Tasks;

namespace LetPortal.Portal.Services.Components
{
    public interface IDynamicListService
    {
        Task<DynamicListResponseDataModel> FetchData(DynamicList dynamicList, DynamicListFetchDataModel fetchDataModel);

        Task<PopulateQueryModel> ExtractingQuery(ExtractingQueryModel extractingQuery);
    }
}
