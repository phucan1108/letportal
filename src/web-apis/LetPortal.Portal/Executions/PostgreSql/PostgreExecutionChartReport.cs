using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Models.Charts;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace LetPortal.Portal.Executions.PostgreSql
{
    public class PostgreExecutionChartReport : IExecutionChartReport
    {
        public ConnectionType ConnectionType => ConnectionType.PostgreSQL;

        private readonly IChartReportQueryBuilder _chartReportQueryBuilder;

        private readonly IChartReportProjection _chartReportProjection;

        public PostgreExecutionChartReport(IChartReportQueryBuilder chartReportQueryBuilder, IChartReportProjection chartReportProjection)
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
            using(var mysqlDbConnection = new NpgsqlConnection(databaseConnection.ConnectionString))
            {
                mysqlDbConnection.Open();
                using(var command = new NpgsqlCommand(formattedString, mysqlDbConnection))
                {
                    var chartQuery = _chartReportQueryBuilder
                                            .Init(formattedString, mappingProjection)
                                            .AddFilters(filterValues)
                                            .AddParameters(parameters)
                                            .Build();
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

        private IEnumerable<NpgsqlParameter> ConvertToParameters(List<ChartReportParameter> parameters)
        {
            foreach(var param in parameters)
            {
                var postgreParam = new NpgsqlParameter(param.Name, ConvertTypeToPostgre(param.ValueType))
                {
                    Value = param.CastedValue
                };
                yield return postgreParam;
            }
        }

        private NpgsqlDbType ConvertTypeToPostgre(Type type)
        {
            if(type == typeof(decimal))
            {
                return NpgsqlDbType.Numeric;
            }
            else if(type == typeof(double))
            {
                return NpgsqlDbType.Double;
            }
            else if(type == typeof(long))
            {
                return NpgsqlDbType.Bigint;
            }
            else if(type == typeof(int))
            {
                return NpgsqlDbType.Integer;
            }
            else if(type == typeof(bool))
            {
                return NpgsqlDbType.Boolean;
            }
            else if(type == typeof(DateTime))
            {
                return NpgsqlDbType.Date;
            }
            else
            {
                return NpgsqlDbType.Varchar;
            }
        }
    }
}
