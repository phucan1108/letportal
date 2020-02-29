using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Constants;
using LetPortal.Portal.Entities.Components;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Mappers;
using LetPortal.Portal.Mappers.MySQL;
using LetPortal.Portal.Models.Charts;
using MySql.Data.MySqlClient;

namespace LetPortal.Portal.Executions.MySQL
{
    public class MySqlExtractionChartQuery : IExtractionChartQuery
    {
        public ConnectionType ConnectionType => ConnectionType.MySQL;

        private readonly IMySqlMapper _mySqlMapper;

        private readonly ICSharpMapper _cSharpMapper;

        public MySqlExtractionChartQuery(IMySqlMapper mySqlMapper, ICSharpMapper cSharpMapper)
        {
            _mySqlMapper = mySqlMapper;
            _cSharpMapper = cSharpMapper;
        }

        public Task<ExtractionChartFilter> Extract(
            DatabaseConnection databaseConnection,
            string formattedString,
            IEnumerable<ChartParameterValue> parameterValues)
        {
            var chartFilters = new ExtractionChartFilter
            {
                Filters = new System.Collections.Generic.List<Entities.Components.ChartFilter>()
            };
            using (var mysqlDbConnection = new MySqlConnection(databaseConnection.ConnectionString))
            {
                mysqlDbConnection.Open();
                var warpQuery = @"Select * from ({0}) s limit 1";
                warpQuery = string.Format(warpQuery, formattedString);

                warpQuery = warpQuery.Replace(Constants.REAL_TIME_KEY, "1=1");
                warpQuery = warpQuery.Replace(Constants.FILTER_KEY, "1=1");
                var listParams = new List<MySqlParameter>();
                if (parameterValues != null)
                {
                    foreach (var parameter in parameterValues)
                    {
                        var fieldParam = StringUtil.GenerateUniqueName();
                        formattedString = formattedString.Replace("{{" + parameter.Name + "}}", "@" + fieldParam);
                        listParams.Add(
                            new MySqlParameter(fieldParam, GetMySqlDbType(parameter.Name, parameter.Value, out var castObject))
                            {
                                Value = castObject,
                                Direction = ParameterDirection.Input
                            });
                    }
                }
                using (var command = new MySqlCommand(formattedString, mysqlDbConnection))
                {
                    command.Parameters.AddRange(listParams.ToArray());
                    using (var reader = command.ExecuteReader())
                    {
                        using (var dt = new DataTable())
                        {
                            dt.Load(reader);
                            foreach (DataColumn dc in dt.Columns)
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

        private FilterType GetType(Type type)
        {
            if (type == typeof(DateTime))
            {
                return FilterType.DatePicker;
            }
            else if (type == typeof(int)
                || type == typeof(float)
                || type == typeof(double)
                || type == typeof(decimal)
                || type == typeof(long))
            {
                return FilterType.NumberPicker;
            }
            else if (type == typeof(bool))
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
