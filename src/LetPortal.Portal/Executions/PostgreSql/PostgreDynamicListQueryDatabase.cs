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
using LetPortal.Portal.Mappers.PostgreSql;
using LetPortal.Portal.Models.DynamicLists;
using Newtonsoft.Json;
using Npgsql;

namespace LetPortal.Portal.Executions.PostgreSql
{
    public class PostgreDynamicListQueryDatabase : IDynamicListQueryDatabase
    {
        private readonly IDynamicQueryBuilder _builder;

        private readonly IPostgreSqlMapper _postgreSqlMapper;

        private readonly ICSharpMapper _cSharpMapper;

        public PostgreDynamicListQueryDatabase(
            IDynamicQueryBuilder builder,
            IPostgreSqlMapper postgreSqlMapper,
            ICSharpMapper cSharpMapper)
        {
            _builder = builder;
            _postgreSqlMapper = postgreSqlMapper;
            _cSharpMapper = cSharpMapper;
        }

        public ConnectionType ConnectionType => ConnectionType.PostgreSQL;

        public Task<DynamicListResponseDataModel> Query(DatabaseConnection databaseConnection, DynamicList dynamicList, DynamicListFetchDataModel fetchDataModel)
        {
            var response = new DynamicListResponseDataModel();
            var hasRows = false;
            using (var postgreDbConnection = new NpgsqlConnection(databaseConnection.ConnectionString))
            {
                var combinedQuery =
                    _builder
                        .Init(
                            dynamicList.ListDatasource.DatabaseConnectionOptions.Query,
                            fetchDataModel.FilledParameterOptions.FilledParameters)
                        .AddTextSearch(fetchDataModel.TextSearch, dynamicList.ColumnsList.ColumnDefs.Where(a => a.SearchOptions.AllowTextSearch).Select(b => b.Name))
                        .AddFilter(fetchDataModel.FilterGroupOptions.FilterGroups)
                        .AddSort(fetchDataModel.SortOptions.SortableFields)
                        .AddPagination(fetchDataModel.PaginationOptions.PageNumber, fetchDataModel.PaginationOptions.PageSize)
                        .Build();

                postgreDbConnection.Open();
                using (var cmd = new NpgsqlCommand(combinedQuery.CombinedQuery, postgreDbConnection))
                {
                    foreach (var param in combinedQuery.Parameters)
                    {
                        if (param.IsReplacedValue)
                        {
                            var castObject = _cSharpMapper.GetCSharpObjectByType(param.Value, param.ReplaceValueType);
                            cmd.Parameters.Add(
                                new NpgsqlParameter(
                                    param.Name, _postgreSqlMapper.GetNpgsqlDbType(param.ReplaceValueType))
                                {
                                    Value = castObject,
                                    Direction = System.Data.ParameterDirection.Input
                                });
                        }
                        else
                        {
                            cmd.Parameters.Add(
                              new NpgsqlParameter(
                                  param.Name, GetNpgsqlDbType(param, out var castObject))
                              {
                                  Value = castObject,
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
                    using (var cmd = new NpgsqlCommand(combinedQuery.CombinedTotalQuery, postgreDbConnection))
                    {
                        foreach (var param in combinedQuery.Parameters)
                        {
                            if (param.IsReplacedValue)
                            {
                                var castObject = _cSharpMapper.GetCSharpObjectByType(param.Value, param.ReplaceValueType);
                                cmd.Parameters.Add(
                                    new NpgsqlParameter(
                                        param.Name, _postgreSqlMapper.GetNpgsqlDbType(param.ReplaceValueType))
                                    {
                                        Value = castObject,
                                        Direction = System.Data.ParameterDirection.Input
                                    });
                            }
                            else
                            {
                                cmd.Parameters.Add(
                                  new NpgsqlParameter(
                                      param.Name, GetNpgsqlDbType(param, out var castObject))
                                  {
                                      Value = castObject,
                                      Direction = System.Data.ParameterDirection.Input
                                  });
                            }
                        }

                        response.TotalItems = (int)cmd.ExecuteScalar();
                    }
                }
                postgreDbConnection.Close();
            }

            return Task.FromResult(response);
        }

        private List<FieldFormatCompare> GetFormatFields(List<ColumnDef> columndDefs)
        {
            return columndDefs.Where(a => !string.IsNullOrEmpty(a.DisplayFormat)).Select(b => new FieldFormatCompare { FieldFormat = b.DisplayFormat, FieldName = b.Name }).ToList();
        }

        private NpgsqlTypes.NpgsqlDbType GetNpgsqlDbType(DynamicQueryParameter param, out object castObj)
        {
            switch (param.ValueType)
            {
                case FieldValueType.Number:
                    castObj = _cSharpMapper.GetCSharpObjectByType(param.Value, MapperConstants.Long);
                    return NpgsqlTypes.NpgsqlDbType.Numeric;
                case FieldValueType.DatePicker:
                    castObj = _cSharpMapper.GetCSharpObjectByType(param.Value, MapperConstants.Date);
                    return NpgsqlTypes.NpgsqlDbType.Date;
                case FieldValueType.Checkbox:
                case FieldValueType.Slide:
                    castObj = _cSharpMapper.GetCSharpObjectByType(param.Value, MapperConstants.Bool);
                    return NpgsqlTypes.NpgsqlDbType.Boolean;
                case FieldValueType.Select:
                case FieldValueType.Text:
                default:
                    castObj = param.Value;
                    return NpgsqlTypes.NpgsqlDbType.Text;
            }
        }
    }
}
