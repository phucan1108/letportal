using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Components;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Models.Charts;

namespace LetPortal.Portal.Executions.SqlServer
{
    public class SqlServerExtractionChartQuery : IExtractionChartQuery
    {
        public ConnectionType ConnectionType => ConnectionType.SQLServer;

        public Task<ExtractionChartFilter> Extract(
            DatabaseConnection databaseConnection,
            string formattedString,
            IEnumerable<ChartParameterValue> parameterValues)
        {
            var chartFilters = new ExtractionChartFilter
            {
                Filters = new System.Collections.Generic.List<Entities.Components.ChartFilter>()
            };
            using(var postgreDbConnection = new SqlConnection(databaseConnection.ConnectionString))
            {
                postgreDbConnection.Open();
                var warpQuery = @"Select * from ({0}) s limit 1";
                warpQuery = string.Format(warpQuery, formattedString);
                var listParams = new List<SqlParameter>();
                if(parameterValues != null)
                {
                    foreach(var parameter in parameterValues)
                    {
                        var fieldParam = StringUtil.GenerateUniqueName();
                        formattedString = formattedString.Replace("{{" + parameter.Name + "}}", "@" + fieldParam);
                        object castObject;
                        listParams.Add(
                            new SqlParameter(fieldParam, GetSqlDbType(parameter.Value, out castObject))
                            {
                                Value = castObject,
                                Direction = ParameterDirection.Input
                            });
                    }
                }
                using(var command = new SqlCommand(formattedString, postgreDbConnection))
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
                return SqlDbType.Date;
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
                return FilterType.NumberPicker;
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
