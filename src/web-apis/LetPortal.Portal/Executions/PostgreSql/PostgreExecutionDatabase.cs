using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Constants;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Mappers;
using LetPortal.Portal.Mappers.PostgreSql;
using LetPortal.Portal.Models;
using LetPortal.Portal.Models.Databases;
using Npgsql;
using NpgsqlTypes;

namespace LetPortal.Portal.Executions.PostgreSql
{
    public class PostgreExecutionDatabase : IExecutionDatabase
    {
        public ConnectionType ConnectionType => ConnectionType.PostgreSQL;

        private readonly IPostgreSqlMapper _postgreSqlMapper;

        private readonly ICSharpMapper _cSharpMapper;

        public PostgreExecutionDatabase(IPostgreSqlMapper postgreSqlMapper, ICSharpMapper cSharpMapper)
        {
            _postgreSqlMapper = postgreSqlMapper;
            _cSharpMapper = cSharpMapper;
        }

        public async Task<ExecuteDynamicResultModel> Execute(DatabaseConnection databaseConnection, string formattedString, IEnumerable<ExecuteParamModel> parameters)
        {
            var result = new ExecuteDynamicResultModel();
            using(var postgreDbConnection = new NpgsqlConnection(databaseConnection.ConnectionString))
            {
                postgreDbConnection.Open();
                using(var command = new NpgsqlCommand(formattedString, postgreDbConnection))
                {
                    string upperFormat = formattedString.ToUpper();
                    bool isQuery = upperFormat.Contains("SELECT ") && upperFormat.Contains("FROM ");
                    bool isInsert = upperFormat.Contains("INSERT INTO ");
                    bool isUpdate = upperFormat.Contains("UPDATE ");
                    bool isDelete = upperFormat.Contains("DELETE ");
                    bool isStoreProcedure = upperFormat.StartsWith("CALL");

                    var listParams = new List<NpgsqlParameter>();
                    if(parameters != null)
                    {
                        foreach(var parameter in parameters)
                        {                               
                            var fieldParam = StringUtil.GenerateUniqueName();
                            formattedString = formattedString.Replace("{{" + parameter.Name + "}}", "@" + fieldParam);                            
                            listParams.Add(
                                new NpgsqlParameter(fieldParam, GetNpgsqlDbType(parameter.Name, parameter.ReplaceValue, out object castObject))
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

        private NpgsqlDbType GetNpgsqlDbType(string paramName, string value, out object castObj)
        {
            var splitted = paramName.Split("|");
            if(splitted.Length == 1)
            {
                castObj = _cSharpMapper.GetCSharpObjectByType(value, MapperConstants.String);
                return _postgreSqlMapper.GetNpgsqlDbType(MapperConstants.String);
            }
            else
            {
                castObj = _cSharpMapper.GetCSharpObjectByType(value, splitted[1]);
                return _postgreSqlMapper.GetNpgsqlDbType(splitted[1]);
            }
        }
    }
}
