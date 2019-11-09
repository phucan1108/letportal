using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Constants;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Mappers;
using LetPortal.Portal.Mappers.MySQL;
using LetPortal.Portal.Models;
using LetPortal.Portal.Models.Databases;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

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
            using(var mysqlDbConnection = new MySqlConnection(databaseConnection.ConnectionString))
            {
                mysqlDbConnection.Open();
                using(var command = new MySqlCommand(formattedString, mysqlDbConnection))
                {
                    string upperFormat = formattedString.ToUpper();
                    bool isQuery = upperFormat.Contains("SELECT ") && upperFormat.Contains("FROM ");
                    bool isInsert = upperFormat.Contains("INSERT INTO ");
                    bool isUpdate = upperFormat.Contains("UPDATE ");
                    bool isDelete = upperFormat.Contains("DELETE ");
                    bool isStoreProcedure = upperFormat.StartsWith("EXEC ");

                    var listParams = new List<MySqlParameter>();
                    if(parameters != null)
                    {
                        foreach(var parameter in parameters)
                        {
                            var fieldParam = StringUtil.GenerateUniqueName();
                            formattedString = formattedString.Replace("{{" + parameter.Name + "}}", "@" + fieldParam);
                            listParams.Add(
                                new MySqlParameter(fieldParam, GetMySqlDbType(parameter.Name, parameter.ReplaceValue, out object castObject))
                                {
                                    Value = castObject,
                                    Direction = ParameterDirection.Input
                                });
                        }
                    }

                    command.Parameters.AddRange(listParams.ToArray());
                    command.CommandText = formattedString;
                    if(isQuery)
                    {
                        using(var reader = await command.ExecuteReaderAsync())
                        {
                            using(DataTable dt = new DataTable())
                            {
                                dt.Load(reader);
                                result.IsSuccess = true;
                                if(dt.Rows.Count > 0)
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
                    else if(isInsert || isUpdate || isDelete)
                    {
                        int effectiveCols = await command.ExecuteNonQueryAsync();
                        result.IsSuccess = true;
                    }
                    else if(isStoreProcedure)
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
            if(splitted.Length == 1)
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
    }
}
