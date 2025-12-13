using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Datasources;
using LetPortal.Portal.Executions;
using LetPortal.Portal.Models;
using LetPortal.Portal.Providers.Databases;
using MongoDB.Driver;

namespace LetPortal.Portal.Services.Datasources
{
    public class DatasourceService : IDatasourceService
    {
        private readonly IDatabaseServiceProvider _databaseServiceProvider;

        private readonly IEnumerable<IExtractionDatasource> _extractionDatasources;

        public DatasourceService(
            IDatabaseServiceProvider databaseServiceProvider,
            IEnumerable<IExtractionDatasource> extractionDatasources)
        {
            _databaseServiceProvider = databaseServiceProvider;
            _extractionDatasources = extractionDatasources;
        }

        public async Task<ExecutedDataSourceModel> GetDatasourceService(Datasource datasource)
        {
            var datasourceModels = new List<DatasourceModel>();

            if (datasource.DatasourceType == DatasourceType.Static)
            {
                datasourceModels = ConvertUtil.DeserializeObject<List<DatasourceModel>>(datasource.Query);
            }
            else
            {
                var database = await _databaseServiceProvider.GetOneDatabaseConnectionAsync(datasource.DatabaseId);

                var foundExtractionDatasource = _extractionDatasources.First(a => a.ConnectionType == database.GetConnectionType());

                datasourceModels = await foundExtractionDatasource.ExtractionDatasource(database, datasource.Query, datasource.OutputProjection);
            }

            return new ExecutedDataSourceModel
            {
                DatasourceModels = datasourceModels,
                CanCache = datasource.CanCache
            };
        }
    }
}
