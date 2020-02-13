using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Portal.Mappers;
using LetPortal.Portal.Mappers.MySQL;
using LetPortal.Portal.Models.Charts;
using MySql.Data.MySqlClient;

namespace LetPortal.Portal.Executions.MySQL
{
    public class MySqlExecutionChartReport : IExecutionChartReport
    {
        public ConnectionType ConnectionType => ConnectionType.MySQL;

        private readonly IChartReportQueryBuilder _chartReportQueryBuilder;

        private readonly IChartReportProjection _chartReportProjection;

        private readonly IMySqlMapper _mySqlMapper;

        private readonly ICSharpMapper _cSharpMapper;

        public MySqlExecutionChartReport(
            IChartReportQueryBuilder chartReportQueryBuilder,
            IChartReportProjection chartReportProjection,
            IMySqlMapper mySqlMapper,
            ICSharpMapper cSharpMapper)
        {
            _chartReportQueryBuilder = chartReportQueryBuilder;
            _chartReportProjection = chartReportProjection;
            _mySqlMapper = mySqlMapper;
            _cSharpMapper = cSharpMapper;
        }

        public async Task<ExecutionChartResponseModel> Execute(ExecutionChartReportModel model)
        {
            var result = new ExecutionChartResponseModel();
            using (var mysqlDbConnection = new MySqlConnection(model.DatabaseConnection.ConnectionString))
            {
                mysqlDbConnection.Open();
                using (var command = new MySqlCommand(model.FormattedString, mysqlDbConnection))
                {
                    _chartReportQueryBuilder
                                            .Init(model.FormattedString, model.MappingProjection, options =>
                                            {
                                                options.FieldFormat = "`{0}`";
                                                options.AllowBoolIsInt = true;
                                                options.DateCompare = "date({0}){1}date({2})";
                                                options.MonthCompare = "month({0}){1}month({2})";
                                                options.YearCompare = "year({0}){1}year({2})";
                                            })
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

        private IEnumerable<MySqlParameter> ConvertToParameters(List<ChartReportParameter> parameters)
        {
            foreach (var param in parameters)
            {
                var mysqlParam = new MySqlParameter(param.Name, _mySqlMapper.GetMySqlDbType(param.ValueType))
                {
                    Value = param.CastedValue
                };
                yield return mysqlParam;
            }
        }
    }
}
