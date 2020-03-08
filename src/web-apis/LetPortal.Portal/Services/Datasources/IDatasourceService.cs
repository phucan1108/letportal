using System.Threading.Tasks;
using LetPortal.Portal.Entities.Datasources;
using LetPortal.Portal.Models;

namespace LetPortal.Portal.Services.Datasources
{
    public interface IDatasourceService
    {
        Task<ExecutedDataSourceModel> GetDatasourceService(Datasource datasource);
    }
}
