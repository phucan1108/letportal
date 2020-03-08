using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Constants;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Mappers;
using LetPortal.Portal.Mappers.MySQL;
using LetPortal.Portal.Models.Databases;
using MySql.Data.MySqlClient;

namespace LetPortal.Portal.Executions.MySQL
{
    public class MySqlExtractionDatabase : IExtractionDatabase
    {
        public ConnectionType ConnectionType => ConnectionType.MySQL;

        private readonly IMySqlMapper _mySqlMapper;

        private readonly ICSharpMapper _cSharpMapper;

        public MySqlExtractionDatabase(
            IMySqlMapper mySqlMapper,
            ICSharpMapper cSharpMapper)
        {
            _mySqlMapper = mySqlMapper;
            _cSharpMapper = cSharpMapper;
        }

        public Task<ExtractingSchemaQueryModel> Extract(DatabaseConnection database, string formattedString, IEnumerable<ExecuteParamModel> parameters)
        {
            var paramsList = new List<MySqlParameter>();
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    // We need to detect a parameter type and then re-mapping to db type
                    var splitted = param.Name.Split("|");
                    var paramDbType = MySqlDbType.LongText;
                    object parsedValue;
                    if (splitted.Length == 1)
                    {
                        // Default: string type
                        paramDbType = _mySqlMapper.GetMySqlDbType(MapperConstants.String);
                        parsedValue = param.ReplaceValue;
                    }
                    else
                    {
                        // It must contain 2 words
                        paramDbType = _mySqlMapper.GetMySqlDbType(splitted[1]);
                        parsedValue = _cSharpMapper.GetCSharpObjectByType(param.ReplaceValue, splitted[1]);
                    }

                    var fieldParam = StringUtil.GenerateUniqueName();
                    formattedString = formattedString.Replace("{{" + param.Name + "}}", "@" + fieldParam);
                    paramsList.Add(
                        new MySqlParameter(fieldParam, paramDbType)
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
            using (var mysqlDbConnection = new MySqlConnection(database.ConnectionString))
            {
                mysqlDbConnection.Open();
                var warpQuery = @"Select * from ({0}) s limit 1";
                warpQuery = ReplaceAllConstants(warpQuery);
                warpQuery = string.Format(warpQuery, formattedString);
                using (var command = new MySqlCommand(formattedString, mysqlDbConnection))
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

        private string ReplaceAllConstants(string query)
        {
            query = query.Replace(Constants.REAL_TIME_KEY, Constants.TRUE_COMPARE_KEY);
            query = query.Replace(Constants.FILTER_KEY, Constants.TRUE_COMPARE_KEY);
            query = query.Replace(Constants.FILTER_LIST_KEY, Constants.TRUE_COMPARE_KEY);
            query = query.Replace(Constants.SEARCH_KEY, string.Empty);
            query = query.Replace(Constants.CURRENT_PAGE_KEY, "0");
            query = query.Replace(Constants.PAGE_NUM_KEY, "0");
            query = query.Replace(Constants.PAGE_START_KEY, "0");
            return query;
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
