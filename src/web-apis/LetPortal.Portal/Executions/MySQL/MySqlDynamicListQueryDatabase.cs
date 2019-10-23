using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Models.DynamicLists;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace LetPortal.Portal.Executions.MySQL
{
    public class MySqlDynamicListQueryDatabase : IDynamicListQueryDatabase
    {
        private readonly IDynamicQueryBuilder _builder;
        public MySqlDynamicListQueryDatabase(IDynamicQueryBuilder builder)
        {
            _builder = builder;
        }

        public ConnectionType ConnectionType => ConnectionType.MySQL;

        public Task<DynamicListResponseDataModel> Query(DatabaseConnection databaseConnection, DynamicList dynamicList, DynamicListFetchDataModel fetchDataModel)
        {
            var response = new DynamicListResponseDataModel();
            using(var sqlDbConnection = new MySqlConnection(databaseConnection.ConnectionString))
            {
                var combinedQuery =
                    _builder
                        .Init(
                            dynamicList.ListDatasource.DatabaseConnectionOptions.Query,
                            fetchDataModel.FilledParameterOptions.FilledParameters,
                            options =>
                            {
                                options.ContainsOperatorFormat = "LIKE CONCAT('%', {0},'%')";
                                options.FieldFormat = "`{0}`";
                                options.DateCompareFormat = "date({0}){1}date({2})";
                            })
                        .AddTextSearch(fetchDataModel.TextSearch, dynamicList.ColumnsList.ColumndDefs.Where(a => a.SearchOptions.AllowTextSearch).Select(b => b.Name))
                        .AddFilter(fetchDataModel.FilterGroupOptions.FilterGroups)
                        .AddSort(fetchDataModel.SortOptions.SortableFields)
                        .AddPagination(fetchDataModel.PaginationOptions.PageNumber, fetchDataModel.PaginationOptions.PageSize)
                        .Build();

                sqlDbConnection.Open();
                using(var cmd = new MySqlCommand(combinedQuery.CombinedQuery, sqlDbConnection))
                {
                    foreach(var param in combinedQuery.Parameters)
                    {
                        object castObj;
                        cmd.Parameters.Add(
                            new MySqlParameter(
                                param.Name, GetMySqlDbType(param, out castObj))
                            {
                                Value = castObj,
                                Direction = System.Data.ParameterDirection.Input
                            });
                    }
                    using(var reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        response.Data = ConvertUtil.DeserializeObject<dynamic>(ConvertUtil.SerializeObject(dt, true));
                    }
                }
                sqlDbConnection.Close();
            }

            return Task.FromResult(response);
        }

        private MySqlDbType GetMySqlDbType(DynamicQueryParameter param, out object castObj)
        {
            switch(param.ValueType)
            {
                case FieldValueType.Number:
                    castObj = int.Parse(param.Value);
                    return MySqlDbType.Int64;
                case FieldValueType.DatePicker:
                    DateTime tempDt = DateTime.Parse(param.Value);
                    tempDt = tempDt.Date;
                    castObj = tempDt;
                    return MySqlDbType.DateTime;
                case FieldValueType.Checkbox:
                case FieldValueType.Slide:
                    bool temp = bool.Parse(param.Value);
                    castObj = temp ? 1 : 0;
                    return MySqlDbType.Bit;
                case FieldValueType.Select:
                case FieldValueType.Text:
                default:
                    castObj = param.Value;
                    return MySqlDbType.LongText;
            }
        }
    }
}
