using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Models.DynamicLists;
using Npgsql;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace LetPortal.Portal.Executions.PostgreSql
{
    public class PostgreDynamicListQueryDatabase : IDynamicListQueryDatabase
    {
        private readonly IDynamicQueryBuilder _builder;
        public PostgreDynamicListQueryDatabase(IDynamicQueryBuilder builder)
        {
            _builder = builder;
        }

        public ConnectionType ConnectionType => ConnectionType.PostgreSQL;

        public Task<DynamicListResponseDataModel> Query(DatabaseConnection databaseConnection, DynamicList dynamicList, DynamicListFetchDataModel fetchDataModel)
        {
            var response = new DynamicListResponseDataModel();
            using(var postgreDbConnection = new NpgsqlConnection(databaseConnection.ConnectionString))
            {
                var combinedQuery =
                    _builder
                        .Init(
                            dynamicList.ListDatasource.DatabaseConnectionOptions.Query,
                            fetchDataModel.FilledParameterOptions.FilledParameters)
                        .AddTextSearch(fetchDataModel.TextSearch, dynamicList.ColumnsList.ColumndDefs.Where(a => a.SearchOptions.AllowTextSearch).Select(b => b.Name))
                        .AddFilter(fetchDataModel.FilterGroupOptions.FilterGroups)
                        .AddSort(fetchDataModel.SortOptions.SortableFields)
                        .AddPagination(fetchDataModel.PaginationOptions.PageNumber, fetchDataModel.PaginationOptions.PageSize)
                        .Build();

                postgreDbConnection.Open();
                using(var cmd = new NpgsqlCommand(combinedQuery.CombinedQuery, postgreDbConnection))
                {
                    foreach(var param in combinedQuery.Parameters)
                    {
                        cmd.Parameters.Add(
                            new NpgsqlParameter(
                                param.Name, GetNpgsqlDbType(param.ValueType))
                            {
                                Value = param.Value,
                                Direction = System.Data.ParameterDirection.Input
                            });
                    }
                    using(var reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        response.Data = ConvertUtil.SerializeObject(dt, true);
                    }
                }
                postgreDbConnection.Close();
            }

            return Task.FromResult(response);
        }

        private NpgsqlTypes.NpgsqlDbType GetNpgsqlDbType(FieldValueType fieldValueType)
        {
            switch(fieldValueType)
            {
                case FieldValueType.Number:
                    return NpgsqlTypes.NpgsqlDbType.Numeric;
                case FieldValueType.DatePicker:
                    return NpgsqlTypes.NpgsqlDbType.Date;
                case FieldValueType.Checkbox:
                case FieldValueType.Slide:
                    return NpgsqlTypes.NpgsqlDbType.Boolean;
                case FieldValueType.Select:
                case FieldValueType.Text:
                default:
                    return NpgsqlTypes.NpgsqlDbType.Text;
            }
        }
    }
}
