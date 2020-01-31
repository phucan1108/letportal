using LetPortal.Portal.Entities.Datasources;
using LetPortal.Portal.Models;
using System.Threading.Tasks;

namespace LetPortal.Portal.Services.Datasources
{
    public interface IDatasourceService
    {
        Task<ExecutedDataSourceModel> GetDatasourceService(Datasource datasource);
    }
}
