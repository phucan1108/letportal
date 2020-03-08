using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Constants;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Entities.Shared;
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

        public async Task<ExecuteDynamicResultModel> Execute(
            DatabaseConnection databaseConnection, 
            string formattedString, 
            IEnumerable<ExecuteParamModel> parameters)
        {
            var result = new ExecuteDynamicResultModel();
            using (var postgreDbConnection = new NpgsqlConnection(databaseConnection.ConnectionString))
            {
                postgreDbConnection.Open();
                using var command = new NpgsqlCommand(formattedString, postgreDbConnection);
                var upperFormat = formattedString.ToUpper().Trim();
                var isQuery = upperFormat.StartsWith("SELECT ") && upperFormat.Contains("FROM ");
                var isInsert = upperFormat.StartsWith("INSERT INTO ");
                var isUpdate = upperFormat.StartsWith("UPDATE ");
                var isDelete = upperFormat.StartsWith("DELETE ");
                var isStoreProcedure = upperFormat.StartsWith("CALL");

                var listParams = new List<NpgsqlParameter>();
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        var fieldParam = StringUtil.GenerateUniqueName();
                        formattedString = formattedString.Replace("{{" + parameter.Name + "}}", "@" + fieldParam);
                        listParams.Add(
                            new NpgsqlParameter(fieldParam, GetNpgsqlDbType(parameter.Name, parameter.ReplaceValue, out var castObject))
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
                    using var reader = await command.ExecuteReaderAsync();
                    using var dt = new DataTable();
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

            return result;
        }

        public async Task<StepExecutionResult> ExecuteStep(DatabaseConnection databaseConnection, string formattedString, IEnumerable<ExecuteParamModel> parameters, ExecutionDynamicContext context)
        {
            var result = new StepExecutionResult();
            using (var postgreDbConnection = new NpgsqlConnection(databaseConnection?.ConnectionString))
            {
                postgreDbConnection.Open();
                formattedString = formattedString?.Trim();
                using var command = new NpgsqlCommand
                {
                    Connection = postgreDbConnection
                };

                var upperFormat = formattedString.ToUpper(System.Globalization.CultureInfo.CurrentCulture).Trim();
                var isQuery = upperFormat.StartsWith("SELECT ", System.StringComparison.OrdinalIgnoreCase) && upperFormat.Contains("FROM ", System.StringComparison.Ordinal);
                var isInsert = upperFormat.StartsWith("INSERT INTO ", System.StringComparison.OrdinalIgnoreCase);
                var isUpdate = upperFormat.StartsWith("UPDATE ", System.StringComparison.OrdinalIgnoreCase);
                var isDelete = upperFormat.StartsWith("DELETE ", System.StringComparison.OrdinalIgnoreCase);
                var isStoreProcedure = upperFormat.StartsWith("EXEC ", System.StringComparison.OrdinalIgnoreCase);

                var listParams = new List<NpgsqlParameter>();
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        var fieldParam = StringUtil.GenerateUniqueName();
                        formattedString = formattedString.Replace("{{" + parameter.Name + "}}", "@" + fieldParam, System.StringComparison.Ordinal);
                        listParams.Add(
                            new NpgsqlParameter(fieldParam, GetNpgsqlDbType(parameter.Name, parameter.ReplaceValue, out var castObject))
                            {
                                Value = castObject,
                                Direction = ParameterDirection.Input
                            });
                    }
                }

                command.Parameters.AddRange(listParams.ToArray());
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
                command.CommandText = formattedString;
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities
                if (isQuery)
                {
                    result.ExecutionType = StepExecutionType.Query;
                    using var reader = await command.ExecuteReaderAsync();
                    using var dt = new DataTable();
                    dt.Load(reader);
                    result.IsSuccess = true;
                    if (dt.Rows.Count > 0)
                    {
                        var str = ConvertUtil.SerializeObject(dt, true);
                        var dynamicResult = ConvertUtil.DeserializeObject<dynamic>(str);
                        result.Result = ConvertUtil.GetOneInArray(dynamicResult);
                        result.IsSuccess = true;
                    }
                    else
                    {
                        result.Error = "There are no found records.";
                        result.IsSuccess = false;
                    }
                }
                else if (isInsert || isUpdate || isDelete)
                {
                    result.ExecutionType = isInsert ? StepExecutionType.Insert
                                            : isUpdate ? StepExecutionType.Update
                                            : isDelete ? StepExecutionType.Delete
                                            : StepExecutionType.Query;

                    // Check if the command contains Query for returning value
                    if (formattedString.Contains(" SELECT ", StringComparison.OrdinalIgnoreCase))
                    {
                        using var reader = await command.ExecuteReaderAsync();
                        using var dt = new DataTable();
                        dt.Load(reader);
                        result.IsSuccess = true;
                        if (dt.Rows.Count > 0)
                        {
                            var str = ConvertUtil.SerializeObject(dt, true);
                            var dynamicResult = ConvertUtil.DeserializeObject<dynamic>(str);
                            result.Result = ConvertUtil.GetOneInArray(dynamicResult);
                            result.IsSuccess = true;
                        }
                        else
                        {
                            result.IsSuccess = false;
                        }
                    }
                    else
                    {
                        var effectiveCols = await command.ExecuteNonQueryAsync();
                        result.Result = new { EffectiveCols = effectiveCols };
                        result.IsSuccess = true;
                    }
                }
                else if (isStoreProcedure)
                {
                    // TODO: Will implement later
                }
            }

            return result;
        }

        private NpgsqlDbType GetNpgsqlDbType(string paramName, string value, out object castObj)
        {
            var splitted = paramName.Split("|");
            if (splitted.Length == 1)
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
