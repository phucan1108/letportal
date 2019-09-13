using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetPortal.Portal.Executions
{
    public interface IExtractionDatasource
    {
        ConnectionType ConnectionType { get; } 

        Task<List<DatasourceModel>> ExtractionDatasource(
            DatabaseConnection databaseConnection, 
            string formattedQueryString,
            string outputProjection);
    }
}
