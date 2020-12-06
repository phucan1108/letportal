using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Common;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Constants;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Mappers;
using LetPortal.Portal.Mappers.MySQL;
using LetPortal.Portal.Models.DynamicLists;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace LetPortal.Portal.Executions.MySQL
{
    public class MySqlDynamicListQueryDatabase : IDynamicListQueryDatabase
    {
        private readonly IDynamicQueryBuilder _builder;

        private readonly IMySqlMapper _mySqlMapper;

        private readonly ICSharpMapper _cSharpMapper;

        public MySqlDynamicListQueryDatabase(
            IDynamicQueryBuilder builder,
            IMySqlMapper mySqlMapper,
            ICSharpMapper cSharpMapper)
        {
            _builder = builder;
            _mySqlMapper = mySqlMapper;
            _cSharpMapper = cSharpMapper;
        }

        public ConnectionType ConnectionType => ConnectionType.MySQL;

        public Task<DynamicListResponseDataModel> Query(DatabaseConnection databaseConnection, DynamicList dynamicList, DynamicListFetchDataModel fetchDataModel)
        {
            var response = new DynamicListResponseDataModel();
            var hasRows = false;
            using (var mysqlDbConnection = new MySqlConnection(databaseConnection.ConnectionString))
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
                        .AddTextSearch(fetchDataModel.TextSearch, dynamicList.ColumnsList.ColumnDefs.Where(a => a.SearchOptions.AllowTextSearch).Select(b => b.Name))
                        .AddFilter(fetchDataModel.FilterGroupOptions.FilterGroups)
                        .AddSort(fetchDataModel.SortOptions.SortableFields)
                        .AddPagination(fetchDataModel.PaginationOptions.PageNumber, fetchDataModel.PaginationOptions.PageSize)
                        .Build();

                mysqlDbConnection.Open();

                using (var cmd = new MySqlCommand(combinedQuery.CombinedQuery, mysqlDbConnection))
                {
                    foreach (var param in combinedQuery.Parameters)
                    {
                        if (param.IsReplacedValue)
                        {
                            var castObject = _cSharpMapper.GetCSharpObjectByType(param.Value, param.ReplaceValueType);
                            cmd.Parameters.Add(
                                new MySqlParameter(
                                    param.Name, _mySqlMapper.GetMySqlDbType(param.ReplaceValueType))
                                {
                                    Value = castObject,
                                    Direction = System.Data.ParameterDirection.Input
                                });
                        }
                        else
                        {
                            cmd.Parameters.Add(
                                new MySqlParameter(
                                    param.Name, GetMySqlDbType(param, out var castObj))
                                {
                                    Value = castObj,
                                    Direction = System.Data.ParameterDirection.Input
                                });
                        }

                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        using (var dt = new DataTable())
                        {
                            dt.Load(reader);
                            if (dt.Rows.Count > 0)
                            {
                                hasRows = true;
                                response.Data = JsonConvert.DeserializeObject<dynamic>(ConvertUtil.SerializeObject(dt, true), new ArrayConverter(GetFormatFields(dynamicList.ColumnsList.ColumnDefs)));
                            }
                        }

                    }
                }

                if (fetchDataModel.PaginationOptions.NeedTotalItems && hasRows)
                {
                    using (var cmd = new MySqlCommand(combinedQuery.CombinedTotalQuery, mysqlDbConnection))
                    {
                        foreach (var param in combinedQuery.Parameters)
                        {
                            if (param.IsReplacedValue)
                            {
                                var castObject = _cSharpMapper.GetCSharpObjectByType(param.Value, param.ReplaceValueType);
                                cmd.Parameters.Add(
                                    new MySqlParameter(
                                        param.Name, _mySqlMapper.GetMySqlDbType(param.ReplaceValueType))
                                    {
                                        Value = castObject,
                                        Direction = System.Data.ParameterDirection.Input
                                    });
                            }
                            else
                            {
                                cmd.Parameters.Add(
                                    new MySqlParameter(
                                        param.Name, GetMySqlDbType(param, out var castObj))
                                    {
                                        Value = castObj,
                                        Direction = System.Data.ParameterDirection.Input
                                    });
                            }

                        }
                        response.TotalItems = (long)cmd.ExecuteScalar();
                    }
                }
                mysqlDbConnection.Close();
            }

            return Task.FromResult(response);
        }

        private List<FieldFormatCompare> GetFormatFields(List<ColumnDef> columndDefs)
        {
            return columndDefs.Where(a => !string.IsNullOrEmpty(a.DisplayFormat)).Select(b => new FieldFormatCompare { FieldFormat = b.DisplayFormat, FieldName = b.Name }).ToList();
        }

        private MySqlDbType GetMySqlDbType(DynamicQueryParameter param, out object castObj)
        {
            switch (param.ValueType)
            {
                case FieldValueType.Number:
                    castObj = _cSharpMapper.GetCSharpObjectByType(param.Value, MapperConstants.Long);
                    return _mySqlMapper.GetMySqlDbType(MapperConstants.Long);
                case FieldValueType.DatePicker:
                    castObj = _cSharpMapper.GetCSharpObjectByType(param.Value, MapperConstants.Date);
                    return _mySqlMapper.GetMySqlDbType(MapperConstants.Date);
                case FieldValueType.Checkbox:
                case FieldValueType.Slide:
                    var temp = bool.Parse(param.Value);
                    castObj = temp ? 1 : 0;
                    return _mySqlMapper.GetMySqlDbType(MapperConstants.Bool);
                case FieldValueType.Select:
                case FieldValueType.Text:
                default:
                    castObj = param.Value;
                    return _mySqlMapper.GetMySqlDbType(MapperConstants.String);
            }
        }
    }
}
