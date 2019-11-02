using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.Components;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Models.Charts;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace LetPortal.Portal.Executions.PostgreSql
{
    public class PostgreExtractionChartQuery : IExtractionChartQuery
    {
        public ConnectionType ConnectionType => ConnectionType.PostgreSQL;

        public Task<ExtractionChartFilter> Extract(
            DatabaseConnection databaseConnection,
            string formattedString,
            IEnumerable<ChartParameterValue> parameterValues)
        {
            var chartFilters = new ExtractionChartFilter
            {
                Filters = new System.Collections.Generic.List<Entities.Components.ChartFilter>()
            };
            using(var postgreDbConnection = new NpgsqlConnection(databaseConnection.ConnectionString))
            {
                postgreDbConnection.Open();
                var warpQuery = @"Select * from ({0}) s limit 1";
                warpQuery = string.Format(warpQuery, formattedString);
                var listParams = new List<NpgsqlParameter>();
                if(parameterValues != null)
                {
                    foreach(var parameter in parameterValues)
                    {
                        var fieldParam = StringUtil.GenerateUniqueName();
                        formattedString = formattedString.Replace("{{" + parameter.Name + "}}", "@" + fieldParam);
                        object castObject;
                        listParams.Add(
                            new NpgsqlParameter(fieldParam, GetPostgreDbType(parameter.Value, out castObject))
                            {
                                Value = castObject,
                                Direction = ParameterDirection.Input
                            });
                    }
                }
                using(var command = new NpgsqlCommand(formattedString, postgreDbConnection))
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

        private NpgsqlDbType GetPostgreDbType(string value, out object castObj)
        {
            if(decimal.TryParse(value, out decimal tempDecimal))
            {
                castObj = tempDecimal;
                return NpgsqlDbType.Numeric;
            }
            else if(long.TryParse(value, out long tempLong))
            {
                castObj = tempLong;
                return NpgsqlDbType.Bigint;
            }
            else if(int.TryParse(value, out int tempInt))
            {
                castObj = tempInt;
                return NpgsqlDbType.Integer;
            }
            else if(DateTime.TryParse(value, out DateTime tempDateTime))
            {
                castObj = tempDateTime;
                return NpgsqlDbType.Date;
            }
            else if(bool.TryParse(value, out bool tempBool))
            {
                castObj = tempBool;
                return NpgsqlDbType.Boolean;
            }
            else if(TimeSpan.TryParse(value, out TimeSpan tempTimeSpan))
            {
                castObj = tempTimeSpan;
                return NpgsqlDbType.Timestamp;
            }
            else
            {
                castObj = value;
                return NpgsqlDbType.Text;
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
