using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Models.DynamicLists;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetPortal.Portal.Executions.SqlServer
{
    public class SqlServerDynamicListQueryDatabase : IDynamicListQueryDatabase
    {
        private readonly IDynamicQueryBuilder _builder;
        public SqlServerDynamicListQueryDatabase(IDynamicQueryBuilder builder)
        {
            _builder = builder;
        }

        public ConnectionType ConnectionType => ConnectionType.SQLServer;

        public Task<DynamicListResponseDataModel> Query(DatabaseConnection databaseConnection, DynamicList dynamicList, DynamicListFetchDataModel fetchDataModel)
        {
            var response = new DynamicListResponseDataModel();
            using(var sqlDbConnection = new SqlConnection(databaseConnection.ConnectionString))
            {
                var combinedQuery =
                    _builder
                        .Init(
                            dynamicList.ListDatasource.DatabaseConnectionOptions.Query,
                            fetchDataModel.FilledParameterOptions.FilledParameters,
                            options =>
                            {
                                options.ContainsOperatorFormat = " LIKE '%' + {0} + '%'";
                                options.PaginationFormat = "OFFSET {1} ROWS FETCH NEXT {0} ROWS ONLY";
                                options.FieldFormat = "[{0}]";
                            })
                        .AddTextSearch(fetchDataModel.TextSearch, dynamicList.ColumnsList.ColumndDefs.Where(a => a.SearchOptions.AllowTextSearch).Select(b => b.Name))
                        .AddFilter(fetchDataModel.FilterGroupOptions.FilterGroups)
                        .AddSort(fetchDataModel.SortOptions.SortableFields)
                        .AddPagination(fetchDataModel.PaginationOptions.PageNumber, fetchDataModel.PaginationOptions.PageSize)
                        .Build();

                sqlDbConnection.Open();
                using(var cmd = new SqlCommand(combinedQuery.CombinedQuery, sqlDbConnection))
                {
                    foreach(var param in combinedQuery.Parameters)
                    {
                        cmd.Parameters.Add(
                            new SqlParameter(
                                param.Name, GetSqlDbType(param.ValueType))
                            {
                                Value = param.Value,
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

        private SqlDbType GetSqlDbType(FieldValueType fieldValueType)
        {
            switch(fieldValueType)
            {
                case FieldValueType.Number:
                    return SqlDbType.Int;
                case FieldValueType.DatePicker:
                    return SqlDbType.Date;
                case FieldValueType.Checkbox:
                case FieldValueType.Slide:
                    return SqlDbType.Bit;
                case FieldValueType.Select:
                case FieldValueType.Text:
                default:
                    return SqlDbType.NVarChar;
            }
        }
    }
}
