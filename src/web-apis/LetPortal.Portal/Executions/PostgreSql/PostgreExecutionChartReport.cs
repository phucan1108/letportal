using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Mappers;
using LetPortal.Portal.Mappers.PostgreSql;
using LetPortal.Portal.Models.Charts;
using Npgsql;

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

        public async Task<ExecutionChartResponseModel> Execute(ExecutionChartReportModel model)
        {
            var result = new ExecutionChartResponseModel();
            using (var postgreDbConnection = new NpgsqlConnection(model.DatabaseConnection.ConnectionString))
            {
                postgreDbConnection.Open();
                using (var command = new NpgsqlCommand(model.FormattedString, postgreDbConnection))
                {
                    _chartReportQueryBuilder
                                            .Init(model.FormattedString, model.MappingProjection)
                                            .AddFilters(model.FilterValues)
                                            .AddParameters(model.Parameters)
                                            .AddMapper((a, b) =>
                                            {
                                                return _cSharpMapper.GetCSharpObjectByType(a, b);
                                            });

                    if (model.IsRealTime && model.LastComparedDate.HasValue && !string.IsNullOrEmpty(model.ComparedRealTimeField))
                    {
                        _chartReportQueryBuilder.AddRealTime(model.ComparedRealTimeField, model.LastComparedDate.Value, DateTime.UtcNow);
                    }

                    var chartQuery = _chartReportQueryBuilder.Build();

                    command.CommandText = chartQuery.CombinedQuery;

                    if (chartQuery.DbParameters.Count > 0)
                    {
                        command.Parameters.AddRange(ConvertToParameters(chartQuery.DbParameters).ToArray());
                    }
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        using (var dt = new DataTable())
                        {
                            dt.Load(reader);
                            result.IsSuccess = true;
                            if (dt.Rows.Count > 0)
                            {
                                result.Result = await _chartReportProjection.ProjectionFromDataTable(dt, model.MappingProjection);
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
            foreach (var param in parameters)
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
