using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.Databases;
using LetPortal.Portal.Mappers;
using LetPortal.Portal.Mappers.PostgreSql;
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

        private readonly IPostgreSqlMapper _postgreSqlMapper;

        private readonly ICSharpMapper _cSharpMapper;

        public PostgreExecutionChartReport(
            IChartReportQueryBuilder chartReportQueryBuilder, 
            IChartReportProjection chartReportProjection,
            IPostgreSqlMapper postgreSqlMapper,
            ICSharpMapper cSharpMapper)
        {
            _chartReportQueryBuilder = chartReportQueryBuilder;
            _chartReportProjection = chartReportProjection;
            _postgreSqlMapper = postgreSqlMapper;
            _cSharpMapper = cSharpMapper;
        }

        public async Task<ExecutionChartResponseModel> Execute(
            DatabaseConnection databaseConnection,
            string formattedString,
            string mappingProjection,
            IEnumerable<ChartParameterValue> parameters,
            IEnumerable<ChartFilterValue> filterValues)
        {
            var result = new ExecutionChartResponseModel();
            using(var postgreDbConnection = new NpgsqlConnection(databaseConnection.ConnectionString))
            {
                postgreDbConnection.Open();
                using(var command = new NpgsqlCommand(formattedString, postgreDbConnection))
                {
                    var chartQuery = _chartReportQueryBuilder
                                            .Init(formattedString, mappingProjection)
                                            .AddFilters(filterValues)
                                            .AddParameters(parameters)
                                            .AddMapper((a,b) =>
                                            {
                                                return _cSharpMapper.GetCSharpObjectByType(a, b);
                                            })
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
                var postgreParam = new NpgsqlParameter(param.Name, _postgreSqlMapper.GetNpgsqlDbType(param.ValueType))
                {
                    Value = param.CastedValue
                };
                yield return postgreParam;
            }
        }
    }
}
