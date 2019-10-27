using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Models.Charts;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace LetPortal.Portal.Executions.MySQL
{
    public class MySqlExecutionChartReport : IExecutionChartReport
    {
        public ConnectionType ConnectionType => ConnectionType.MySQL;

        private readonly IChartReportQueryBuilder _chartReportQueryBuilder;

        private readonly IChartReportProjection _chartReportProjection;

        public MySqlExecutionChartReport(IChartReportQueryBuilder chartReportQueryBuilder, IChartReportProjection chartReportProjection)
        {
            _chartReportQueryBuilder = chartReportQueryBuilder;
            _chartReportProjection = chartReportProjection;
        }

        public async Task<ExecutionChartResponseModel> Execute(
            DatabaseConnection databaseConnection,
            string formattedString,
            string mappingProjection,
            IEnumerable<ChartParameterValue> parameters,
            IEnumerable<ChartFilterValue> filterValues)
        {
            var result = new ExecutionChartResponseModel();
            using(var mysqlDbConnection = new MySqlConnection(databaseConnection.ConnectionString))
            {
                mysqlDbConnection.Open();
                using(var command = new MySqlCommand(formattedString, mysqlDbConnection))
                {
                    var chartQuery = _chartReportQueryBuilder
                                            .Init(formattedString, mappingProjection, options =>
                                            {
                                                options.FieldFormat = "`{0}`";
                                                options.AllowBoolIsInt = true;
                                                options.DateCompare = "date({0}){1}date({2})";
                                                options.MonthCompare = "month({0}){1}month({2})";
                                                options.YearCompare = "year({0}){1}year({2})";

                                            }).Build();
                    command.CommandText = chartQuery.CombinedQuery;

                    if(chartQuery.DbParameters.Count > 0)
                    {
                        command.Parameters.AddRange(ConvertToParameters(chartQuery.DbParameters).ToArray());
                    }
                    using(var reader = await command.ExecuteReaderAsync())
                    {
                        using(DataTable dt = new DataTable())
                        {
                            dt.Load(reader);
                            result.IsSuccess = true;
                            if(dt.Rows.Count > 0)
                            {
                                result.Result = await _chartReportProjection.ProjectionFromDataTable(dt, mappingProjection);
                                result.IsSuccess = true;
                            }
                            else
                            {
                                result.IsSuccess = false;
                            }
                        }
                    }
                }
            }

            return result;
        }

        private IEnumerable<MySqlParameter> ConvertToParameters(List<ChartReportParameter> parameters)
        {
            foreach(var param in parameters)
            {
                var mysqlParam = new MySqlParameter(param.Name, ConvertTypeToMySql(param.ValueType))
                {
                    Value = param.CastedValue
                };
                yield return mysqlParam;
            }
        }

        private MySqlDbType ConvertTypeToMySql(Type type)
        {
            if(type == typeof(decimal))
            {
                return MySqlDbType.Decimal;
            }
            else if(type == typeof(double))
            {
                return MySqlDbType.Double;
            }
            else if(type == typeof(long))
            {
                return MySqlDbType.Int64;
            }
            else if(type == typeof(int))
            {
                return MySqlDbType.Int32;
            }
            else if(type == typeof(bool))
            {
                return MySqlDbType.Bit;
            }
            else if(type == typeof(DateTime))
            {
                return MySqlDbType.DateTime;
            }
            else
            {
                return MySqlDbType.VarChar;
            }
        }
    }
}
