using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Constants;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Mappers;
using LetPortal.Portal.Mappers.SqlServer;
using LetPortal.Portal.Models.Databases;

namespace LetPortal.Portal.Executions.SqlServer
{
    public class SqlServerExtractionDatabase : IExtractionDatabase
    {
        public ConnectionType ConnectionType => ConnectionType.SQLServer;

        private readonly ISqlServerMapper _sqlServerMapper;

        private readonly ICSharpMapper _cSharpMapper;

        public SqlServerExtractionDatabase(ISqlServerMapper sqlServerMapper, ICSharpMapper cSharpMapper)
        {
            _sqlServerMapper = sqlServerMapper;
            _cSharpMapper = cSharpMapper;
        }

        public Task<ExtractingSchemaQueryModel> Extract(DatabaseConnection database, string formattedString, IEnumerable<ExecuteParamModel> parameters)
        {
            var paramsList = new List<SqlParameter>();
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    // We need to detect a parameter type and then re-mapping to db type
                    var splitted = param.Name.Split("|");
                    var paramDbType = SqlDbType.NVarChar;
                    object parsedValue;
                    if (splitted.Length == 1)
                    {
                        // Default: string type
                        paramDbType = _sqlServerMapper.GetSqlDbType(MapperConstants.String);
                        parsedValue = param.ReplaceValue;
                    }
                    else
                    {
                        // It must contain 2 words
                        paramDbType = _sqlServerMapper.GetSqlDbType(splitted[1]);
                        parsedValue = _cSharpMapper.GetCSharpObjectByType(param.ReplaceValue, splitted[1]);
                    }

                    var fieldParam = StringUtil.GenerateUniqueName();
                    formattedString = formattedString.Replace("{{" + param.Name + "}}", "@" + fieldParam);
                    paramsList.Add(
                        new SqlParameter(fieldParam, paramDbType)
                        {
                            Value = parsedValue,
                            Direction = ParameterDirection.Input
                        });
                }
            }

            var extractModel = new ExtractingSchemaQueryModel
            {
                ColumnFields = new System.Collections.Generic.List<Models.Shared.ColumnField>()
            };
            using (var sqlDbConnection = new SqlConnection(database.ConnectionString))
            {
                sqlDbConnection.Open();
                var warpQuery = @"Select * from ({0}) s limit 1";
                warpQuery = string.Format(warpQuery, formattedString);
                using (var command = new SqlCommand(formattedString, sqlDbConnection))
                {
                    if (paramsList.Count > 0)
                    {
                        command.Parameters.AddRange(paramsList.ToArray());
                    }
                    using (var reader = command.ExecuteReader())
                    {
                        using (var dt = new DataTable())
                        {
                            dt.Load(reader);
                            foreach (DataColumn dc in dt.Columns)
                            {
                                extractModel.ColumnFields.Add(new Models.Shared.ColumnField
                                {
                                    Name = dc.ColumnName,
                                    DisplayName = dc.ColumnName,
                                    FieldType = GetType(dc.DataType)
                                });
                            }
                        }
                    }
                }
            }

            return Task.FromResult(extractModel);
        }

        private string GetType(Type type)
        {
            if (type == typeof(DateTime))
            {
                return "datetime";
            }
            else if (type == typeof(int)
                || type == typeof(float)
                || type == typeof(double)
                || type == typeof(decimal)
                || type == typeof(long))
            {
                return "number";
            }
            else if (type == typeof(bool))
            {
                return "boolean";
            }
            else
            {
                return "string";
            }
        }
    }
}
