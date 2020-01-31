using System.Threading.Tasks;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Models.DynamicLists;

namespace LetPortal.Portal.Services.Components
{
    public interface IDynamicListService
    {
        Task<DynamicListResponseDataModel> FetchData(DynamicList dynamicList, DynamicListFetchDataModel fetchDataModel);

        Task<PopulateQueryModel> ExtractingQuery(ExtractingQueryModel extractingQuery);
    }
}
