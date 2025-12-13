using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.Portal.Constants;
using LetPortal.Portal.Entities.Components;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Mappers;
using LetPortal.Portal.Mappers.SqlServer;
using LetPortal.Portal.Models.Charts;

namespace LetPortal.Portal.Executions.SqlServer
{
    public class SqlServerExtractionChartQuery : IExtractionChartQuery
    {
        public ConnectionType ConnectionType => ConnectionType.SQLServer;

        private readonly ISqlServerMapper _sqlServerMapper;

        private readonly ICSharpMapper _cSharpMapper;

        public SqlServerExtractionChartQuery(ISqlServerMapper sqlServerMapper, ICSharpMapper cSharpMapper)
        {
            _sqlServerMapper = sqlServerMapper;
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
            using (var postgreDbConnection = new SqlConnection(databaseConnection.ConnectionString))
            {
                postgreDbConnection.Open();
                var warpQuery = @"Select * from ({0}) s limit 1";
                warpQuery = string.Format(warpQuery, formattedString);

                warpQuery = warpQuery.Replace(Constants.REAL_TIME_KEY, "1=1");
                warpQuery = warpQuery.Replace(Constants.FILTER_KEY, "1=1");
                var listParams = new List<SqlParameter>();
                if (parameterValues != null)
                {
                    foreach (var parameter in parameterValues)
                    {
                        var fieldParam = StringUtil.GenerateUniqueName();
                        formattedString = formattedString.Replace("{{" + parameter.Name + "}}", "@" + fieldParam);
                        listParams.Add(
                            new SqlParameter(fieldParam, GetSqlDbType(parameter.Name, parameter.Value, out var castObject))
                            {
                                Value = castObject,
                                Direction = ParameterDirection.Input
                            });
                    }
                }
                using (var command = new SqlCommand(formattedString, postgreDbConnection))
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

        private SqlDbType GetSqlDbType(string paramName, string value, out object castObj)
        {
            var splitted = paramName.Split("|");
            if (splitted.Length == 1)
            {
                castObj = _cSharpMapper.GetCSharpObjectByType(value, MapperConstants.String);
                return _sqlServerMapper.GetSqlDbType(MapperConstants.String);
            }
            else
            {
                castObj = _cSharpMapper.GetCSharpObjectByType(value, splitted[1]);
                return _sqlServerMapper.GetSqlDbType(splitted[1]);
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
