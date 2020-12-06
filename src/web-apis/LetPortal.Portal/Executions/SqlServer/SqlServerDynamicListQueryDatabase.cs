using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Common;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Constants;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Mappers;
using LetPortal.Portal.Mappers.SqlServer;
using LetPortal.Portal.Models.DynamicLists;
using Newtonsoft.Json;

namespace LetPortal.Portal.Executions.SqlServer
{
    public class SqlServerDynamicListQueryDatabase : IDynamicListQueryDatabase
    {
        private readonly IDynamicQueryBuilder _builder;

        private readonly ISqlServerMapper _sqlServerMapper;

        private readonly ICSharpMapper _cSharpMapper;

        public SqlServerDynamicListQueryDatabase(
            IDynamicQueryBuilder builder,
            ISqlServerMapper sqlServerMapper,
            ICSharpMapper cSharpMapper)
        {
            _builder = builder;
            _sqlServerMapper = sqlServerMapper;
            _cSharpMapper = cSharpMapper;
        }

        public ConnectionType ConnectionType => ConnectionType.SQLServer;

        public Task<DynamicListResponseDataModel> Query(DatabaseConnection databaseConnection, DynamicList dynamicList, DynamicListFetchDataModel fetchDataModel)
        {
            var response = new DynamicListResponseDataModel();
            var hasRows = false;
            using (var sqlDbConnection = new SqlConnection(databaseConnection.ConnectionString))
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
                                options.DateCompareFormat = "cast({0} as date){1}cast({2} as date)";
                            })
                        .AddTextSearch(fetchDataModel.TextSearch, dynamicList.ColumnsList.ColumnDefs.Where(a => a.SearchOptions.AllowTextSearch).Select(b => b.Name))
                        .AddFilter(fetchDataModel.FilterGroupOptions.FilterGroups)
                        .AddSort(fetchDataModel.SortOptions.SortableFields)
                        .AddPagination(fetchDataModel.PaginationOptions.PageNumber, fetchDataModel.PaginationOptions.PageSize)
                        .Build();

                sqlDbConnection.Open();
                using (var cmd = new SqlCommand(combinedQuery.CombinedQuery, sqlDbConnection))
                {
                    foreach (var param in combinedQuery.Parameters)
                    {
                        if (param.IsReplacedValue)
                        {
                            var castObject = _cSharpMapper.GetCSharpObjectByType(param.Value, param.ReplaceValueType);
                            cmd.Parameters.Add(
                                new SqlParameter(
                                    param.Name, _sqlServerMapper.GetSqlDbType(param.ReplaceValueType))
                                {
                                    Value = castObject,
                                    Direction = System.Data.ParameterDirection.Input
                                });
                        }
                        else
                        {
                            cmd.Parameters.Add(
                              new SqlParameter(
                                  param.Name, GetSqlDbType(param, out var castObject))
                              {
                                  Value = castObject,
                                  Direction = System.Data.ParameterDirection.Input
                              });
                        }
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        var dt = new DataTable();
                        dt.Load(reader);
                        if (dt.Rows.Count > 0)
                        {
                            hasRows = true;
                            response.Data = JsonConvert.DeserializeObject<dynamic>(ConvertUtil.SerializeObject(dt, true), new ArrayConverter(GetFormatFields(dynamicList.ColumnsList.ColumnDefs)));
                        }
                        dt.Dispose();
                    }
                }

                if (fetchDataModel.PaginationOptions.NeedTotalItems && hasRows)
                {
                    using (var cmd = new SqlCommand(combinedQuery.CombinedTotalQuery, sqlDbConnection))
                    {
                        foreach (var param in combinedQuery.Parameters)
                        {
                            if (param.IsReplacedValue)
                            {
                                var castObject = _cSharpMapper.GetCSharpObjectByType(param.Value, param.ReplaceValueType);
                                cmd.Parameters.Add(
                                    new SqlParameter(
                                        param.Name, _sqlServerMapper.GetSqlDbType(param.ReplaceValueType))
                                    {
                                        Value = castObject,
                                        Direction = System.Data.ParameterDirection.Input
                                    });
                            }
                            else
                            {
                                cmd.Parameters.Add(
                                  new SqlParameter(
                                      param.Name, GetSqlDbType(param, out var castObject))
                                  {
                                      Value = castObject,
                                      Direction = System.Data.ParameterDirection.Input
                                  });
                            }
                        }
                        response.TotalItems = (int)cmd.ExecuteScalar();
                    }
                }
                sqlDbConnection.Close();
            }

            return Task.FromResult(response);
        }

        private List<FieldFormatCompare> GetFormatFields(List<ColumnDef> columndDefs)
        {
            return columndDefs.Where(a => !string.IsNullOrEmpty(a.DisplayFormat)).Select(b => new FieldFormatCompare { FieldFormat = b.DisplayFormat, FieldName = b.Name }).ToList();
        }

        private SqlDbType GetSqlDbType(DynamicQueryParameter param, out object castObj)
        {
            switch (param.ValueType)
            {
                case FieldValueType.Number:
                    castObj = _cSharpMapper.GetCSharpObjectByType(param.Value, MapperConstants.Long);
                    return SqlDbType.Int;
                case FieldValueType.DatePicker:
                    castObj = _cSharpMapper.GetCSharpObjectByType(param.Value, MapperConstants.Date);
                    return SqlDbType.Date;
                case FieldValueType.Checkbox:
                case FieldValueType.Slide:
                    castObj = _cSharpMapper.GetCSharpObjectByType(param.Value, MapperConstants.Bool);
                    return SqlDbType.Bit;
                case FieldValueType.Select:
                case FieldValueType.Text:
                default:
                    castObj = param.Value;
                    return SqlDbType.NVarChar;
            }
        }
    }
}
