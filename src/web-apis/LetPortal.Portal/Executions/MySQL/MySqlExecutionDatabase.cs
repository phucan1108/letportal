using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Constants;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Entities.Shared;
using LetPortal.Portal.Mappers;
using LetPortal.Portal.Mappers.MySQL;
using LetPortal.Portal.Models;
using LetPortal.Portal.Models.Databases;
using MySql.Data.MySqlClient;

namespace LetPortal.Portal.Executions.MySQL
{
    public class MySqlExecutionDatabase : IExecutionDatabase
    {
        public ConnectionType ConnectionType => ConnectionType.MySQL;

        private readonly IMySqlMapper _mySqlMapper;

        private readonly ICSharpMapper _cSharpMapper;

        public MySqlExecutionDatabase(IMySqlMapper mySqlMapper, ICSharpMapper cSharpMapper)
        {
            _mySqlMapper = mySqlMapper;
            _cSharpMapper = cSharpMapper;
        }

        public async Task<ExecuteDynamicResultModel> Execute(DatabaseConnection databaseConnection, string formattedString, IEnumerable<ExecuteParamModel> parameters)
        {
            var result = new ExecuteDynamicResultModel();
            using (var mysqlDbConnection = new MySqlConnection(databaseConnection.ConnectionString))
            {
                mysqlDbConnection.Open();
                using (var command = new MySqlCommand(formattedString, mysqlDbConnection))
                {
                    var upperFormat = formattedString.ToUpper().Trim();
                    var isQuery = upperFormat.StartsWith("SELECT ") && upperFormat.Contains("FROM ");
                    var isInsert = upperFormat.StartsWith("INSERT INTO ");
                    var isUpdate = upperFormat.StartsWith("UPDATE ");
                    var isDelete = upperFormat.StartsWith("DELETE ");
                    var isStoreProcedure = upperFormat.StartsWith("EXEC ");

                    var listParams = new List<MySqlParameter>();
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            var fieldParam = StringUtil.GenerateUniqueName();
                            formattedString = formattedString.Replace("{{" + parameter.Name + "}}", "@" + fieldParam);
                            listParams.Add(
                                new MySqlParameter(fieldParam, GetMySqlDbType(parameter.Name, parameter.ReplaceValue, out var castObject))
                                {
                                    Value = castObject,
                                    Direction = ParameterDirection.Input
                                });
                        }
                    }

                    command.Parameters.AddRange(listParams.ToArray());
                    command.CommandText = formattedString;
                    if (isQuery)
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            using (var dt = new DataTable())
                            {
                                dt.Load(reader);
                                result.IsSuccess = true;
                                if (dt.Rows.Count > 0)
                                {
                                    var str = ConvertUtil.SerializeObject(dt, true);
                                    result.Result = ConvertUtil.DeserializeObject<dynamic>(str);
                                    result.IsSuccess = true;
                                }
                                else
                                {
                                    result.IsSuccess = false;
                                }
                            }
                        }
                    }
                    else if (isInsert || isUpdate || isDelete)
                    {
                        var effectiveCols = await command.ExecuteNonQueryAsync();
                        result.IsSuccess = true;
                    }
                    else if (isStoreProcedure)
                    {
                        // TODO: Will implement later
                    }
                }
            }

            return result;
        }

        private MySqlDbType GetMySqlDbType(string paramName, string value, out object castObj)
        {
            var splitted = paramName.Split("|");
            if (splitted.Length == 1)
            {
                castObj = _cSharpMapper.GetCSharpObjectByType(value, MapperConstants.String);
                return _mySqlMapper.GetMySqlDbType(MapperConstants.String);
            }
            else
            {
                castObj = _cSharpMapper.GetCSharpObjectByType(value, splitted[1]);
                return _mySqlMapper.GetMySqlDbType(splitted[1]);
            }
        }

        public Task<ExecuteDynamicResultModel> Execute(List<DatabaseConnection> databaseConnections, DatabaseExecutionChains executionChains, IEnumerable<ExecuteParamModel> parameters)
        {
            throw new System.NotImplementedException();
        }
    }
}
