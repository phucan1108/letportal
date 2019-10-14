using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Models;
using LetPortal.Portal.Models.Databases;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace LetPortal.Portal.Executions.SqlServer
{
    public class SqlServerExecutionDatabase : IExecutionDatabase
    {
        public ConnectionType ConnectionType => ConnectionType.SQLServer;

        public async Task<ExecuteDynamicResultModel> Execute(DatabaseConnection databaseConnection, string formattedString, IEnumerable<ExecuteParamModel> parameters)
        {
            var result = new ExecuteDynamicResultModel();
            using(var sqlDbConnection = new SqlConnection(databaseConnection.ConnectionString))
            {
                sqlDbConnection.Open();
                using(var command = new SqlCommand(formattedString, sqlDbConnection))
                {
                    string upperFormat = formattedString.ToUpper();
                    bool isQuery = upperFormat.Contains("SELECT ") && upperFormat.Contains("FROM ");
                    bool isInsert = upperFormat.Contains("INSERT INTO ");
                    bool isUpdate = upperFormat.Contains("UPDATE ");
                    bool isDelete = upperFormat.Contains("DELETE ");
                    bool isStoreProcedure = upperFormat.StartsWith("EXEC ");

                    var listParams = new List<SqlParameter>();
                    if(parameters != null)
                    {
                        foreach(var parameter in parameters)
                        {
                            var fieldParam = StringUtil.GenerateUniqueName();
                            formattedString = formattedString.Replace("{{" + parameter.Name + "}}", "@" + fieldParam);
                            object castObject;
                            listParams.Add(
                                new SqlParameter(fieldParam, GetSqlDbType(parameter.ReplaceValue, out castObject))
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

        private SqlDbType GetSqlDbType(string value, out object castObj)
        {
            if(decimal.TryParse(value, out decimal tempDecimal))
            {
                castObj = tempDecimal;
                return SqlDbType.Decimal;
            }
            else if(long.TryParse(value, out long tempLong))
            {
                castObj = tempLong;
                return SqlDbType.BigInt;
            }
            else if(int.TryParse(value, out int tempInt))
            {
                castObj = tempInt;
                return SqlDbType.Int;
            }
            else if(DateTime.TryParse(value, out DateTime tempDateTime))
            {
                castObj = tempDateTime;
                return SqlDbType.DateTime2;
            }
            else if(bool.TryParse(value, out bool tempBool))
            {
                castObj = tempBool;
                return SqlDbType.Bit;
            }
            else if(TimeSpan.TryParse(value, out TimeSpan tempTimeSpan))
            {
                castObj = tempTimeSpan;
                return SqlDbType.Timestamp;
            }
            else
            {
                castObj = value;
                return SqlDbType.NVarChar;
            }
        }
    }
}
