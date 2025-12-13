using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Mappers;
using LetPortal.Portal.Mappers.SqlServer;
using LetPortal.Portal.Models.Charts;

namespace LetPortal.Portal.Executions.SqlServer
{
    public class SqlServerExecutionChartReport : IExecutionChartReport
    {
        public ConnectionType ConnectionType => ConnectionType.SQLServer;

        private readonly IChartReportQueryBuilder _chartReportQueryBuilder;

        private readonly IChartReportProjection _chartReportProjection;

        private readonly ISqlServerMapper _sqlServerMapper;

        private readonly ICSharpMapper _cSharpMapper;

        public SqlServerExecutionChartReport(
            IChartReportQueryBuilder chartReportQueryBuilder,
            IChartReportProjection chartReportProjection,
            ISqlServerMapper sqlServerMapper,
            ICSharpMapper cSharpMapper)
        {
            _chartReportQueryBuilder = chartReportQueryBuilder;
            _chartReportProjection = chartReportProjection;
            _sqlServerMapper = sqlServerMapper;
            _cSharpMapper = cSharpMapper;
        }

        public async Task<ExecutionChartResponseModel> Execute(ExecutionChartReportModel model)
        {
            var result = new ExecutionChartResponseModel();
            using (var sqlDbConnection = new SqlConnection(model.DatabaseConnection.ConnectionString))
            {
                sqlDbConnection.Open();
                using (var command = new SqlCommand(model.FormattedString, sqlDbConnection))
                {
                    _chartReportQueryBuilder
                                            .Init(model.FormattedString, model.MappingProjection, options =>
                                            {
                                                options.FieldFormat = "[{0}]";
                                                options.AllowBoolIsInt = false;
                                                options.DateCompare = "cast({0} as date){1}cast({2} as date)";
                                                options.MonthCompare = "month({0}){1}month({2})";
                                                options.YearCompare = "year({0}){1}year({2})";
                                            })
                                            .AddFilters(model.FilterValues)
                                            .AddParameters(model.Parameters)
                                            .AddMapper((a, b) =>
                                            {
                                                return _cSharpMapper.GetCSharpObjectByType(a, b);
                                            });

                    // In case we compare exact field and data has been fetched one times
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

        private IEnumerable<SqlParameter> ConvertToParameters(List<ChartReportParameter> parameters)
        {
            foreach (var param in parameters)
            {
                var postgreParam = new SqlParameter(param.Name, _sqlServerMapper.GetSqlDbType(param.ValueType))
                {
                    Value = param.CastedValue
                };
                yield return postgreParam;
            }
        }           
    }
}
