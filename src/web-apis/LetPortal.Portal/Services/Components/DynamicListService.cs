using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Executions;
using LetPortal.Portal.Models.DynamicLists;
using LetPortal.Portal.Providers.Databases;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetPortal.Portal.Services.Components
{
    public class DynamicListService : IDynamicListService
    {
        private readonly IDatabaseServiceProvider _databaseServiceProvider;

        private readonly IEnumerable<IDynamicListQueryDatabase> _dynamicListQueryDatabases;

        public DynamicListService(
            IDatabaseServiceProvider databaseServiceProvider,
            IEnumerable<IDynamicListQueryDatabase> dynamicListQueryDatabases)
        {
            _databaseServiceProvider = databaseServiceProvider;
            _dynamicListQueryDatabases = dynamicListQueryDatabases;
        }

        public async Task<DynamicListResponseDataModel> FetchData(DynamicList dynamicList, DynamicListFetchDataModel fetchDataModel)
        {
            var databaseConnection = await _databaseServiceProvider.GetOneDatabaseConnectionAsync(dynamicList.ListDatasource.DatabaseConnectionOptions.DatabaseConnectionId);
            var foundDynamicListQuery = _dynamicListQueryDatabases.First(a => a.ConnectionType == databaseConnection.GetConnectionType());

            return await foundDynamicListQuery.Query(databaseConnection, dynamicList, fetchDataModel);
        }

        public async Task<PopulateQueryModel> ExtractingQuery(ExtractingQueryModel extractingQuery)
        {
            var query = extractingQuery.Query;
            foreach(var param in extractingQuery.Parameters)
            {
                query = query.Replace("{{" + param.Name + "}}", param.Value);
            }

            var extractingResult = await _databaseServiceProvider.GetSchemasByQuery(extractingQuery.DatabaseId, query);

            return new PopulateQueryModel { ColumnFields = extractingResult.ColumnFields };
        }
    }
}
