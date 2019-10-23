using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Components;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Models.Charts;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LetPortal.Portal.Executions.MySQL
{
    public class MySqlExtractionChartQuery : IExtractionChartQuery
    {
        public ConnectionType ConnectionType => ConnectionType.MySQL;

        public Task<ExtractionChartFilter> Extract(
            DatabaseConnection databaseConnection, 
            string formattedString,
            IEnumerable<ChartParameterValue> parameterValues)
        {
            var chartFilters = new ExtractionChartFilter
            {
                Filters = new System.Collections.Generic.List<Entities.Components.ChartFilter>()
            };
            using(var mysqlDbConnection = new MySqlConnection(databaseConnection.ConnectionString))
            {
                mysqlDbConnection.Open();
                var warpQuery = @"Select * from ({0}) s limit 1";
                warpQuery = string.Format(warpQuery, formattedString);
                var listParams = new List<MySqlParameter>();
                if(parameterValues != null)
                {
                    foreach(var parameter in parameterValues)
                    {
                        var fieldParam = StringUtil.GenerateUniqueName();
                        formattedString = formattedString.Replace("{{" + parameter.Name + "}}", "@" + fieldParam);
                        object castObject;
                        listParams.Add(
                            new MySqlParameter(fieldParam, GetMySqlDbType(parameter.Value, out castObject))
                            {
                                Value = castObject,
                                Direction = ParameterDirection.Input
                            });
                    }
                }
                using(var command = new MySqlCommand(formattedString, mysqlDbConnection))
                {
                    command.Parameters.AddRange(listParams.ToArray());
                    using(var reader = command.ExecuteReader())
                    {
                        using(DataTable dt = new DataTable())
                        {
                            dt.Load(reader);
                            foreach(DataColumn dc in dt.Columns)
                            {
                                chartFilters.Filters.Add(new Entities.Components.ChartFilter
                                {
                                    Name = dc.ColumnName,
                                    DisplayName = dc.ColumnName,
                                    Type = GetType(dc.DataType)
                                });
                            }
                        }
                    }
                }
            }

            return Task.FromResult(chartFilters);
        }

        private MySqlDbType GetMySqlDbType(string value, out object castObj)
        {
            if(decimal.TryParse(value, out decimal tempDecimal))
            {
                castObj = tempDecimal;
                return MySqlDbType.Decimal;
            }
            else if(long.TryParse(value, out long tempLong))
            {
                castObj = tempLong;
                return MySqlDbType.Int64;
            }
            else if(int.TryParse(value, out int tempInt))
            {
                castObj = tempInt;
                return MySqlDbType.Int32;
            }
            else if(DateTime.TryParse(value, out DateTime tempDateTime))
            {
                castObj = tempDateTime;
                return MySqlDbType.DateTime;
            }
            else if(bool.TryParse(value, out bool tempBool))
            {
                castObj = tempBool;
                return MySqlDbType.Bit;
            }
            else if(TimeSpan.TryParse(value, out TimeSpan tempTimeSpan))
            {
                castObj = tempTimeSpan;
                return MySqlDbType.Timestamp;
            }
            else
            {
                castObj = value;
                return MySqlDbType.VarChar;
            }
        }

        private FilterType GetType(Type type)
        {
            if(type == typeof(DateTime))
            {
                return FilterType.DatePicker;
            }
            else if(type == typeof(int)
                || type == typeof(float)
                || type == typeof(double)
                || type == typeof(decimal)
                || type == typeof(long))
            {
                return FilterType.NumberRange;
            }
            else if(type == typeof(bool))
            {
                return FilterType.Checkbox;
            }
            else
            {
                return FilterType.Select;
            }
        }
    }
}
