using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Constants;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Entities.SectionParts;
using LetPortal.Portal.Mappers;
using LetPortal.Portal.Mappers.PostgreSql;
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
                        if(param.IsReplacedValue)
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
                                  param.Name, GetNpgsqlDbType(param, out object castObject))
                              {
                                  Value = castObject,
                                  Direction = System.Data.ParameterDirection.Input
                              });
                        }                        
                    }
                    using(var reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        response.Data = ConvertUtil.DeserializeObject<dynamic>(ConvertUtil.SerializeObject(dt, true));
                    }
                }
                postgreDbConnection.Close();
            }

            return Task.FromResult(response);
        }

        private NpgsqlTypes.NpgsqlDbType GetNpgsqlDbType(DynamicQueryParameter param, out object castObj)
        {
            switch(param.ValueType)
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
