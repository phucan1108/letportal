using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Models.DynamicLists;

namespace LetPortal.Portal.Executions
{
    public interface IDynamicListQueryDatabase
    {
        ConnectionType ConnectionType { get; }

        Task<DynamicListResponseDataModel> Query(DatabaseConnection databaseConnection, DynamicList dynamicList, DynamicListFetchDataModel fetchDataModel);
    }
}
