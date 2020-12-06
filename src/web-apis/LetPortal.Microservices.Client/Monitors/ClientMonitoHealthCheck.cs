using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using LetPortal.Core.Logger;
using LetPortal.Core.Monitors;
using LetPortal.Core.Monitors.Models;
using LetPortal.Core.Persistences;
using LetPortal.Microservices.Monitors;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MySql.Data.MySqlClient;
using Npgsql;

namespace LetPortal.Microservices.Client.Monitors
{
    public class ClientMonitorHealthCheck : IHealthCheck
    {
        private readonly IOptionsMonitor<MonitorOptions> _monitorOptions;
        private readonly IOptionsMonitor<DatabaseOptions> _databaseOptions;
        private readonly IOptionsMonitor<LoggerOptions> _loggerOptions;
        private readonly IOptionsMonitor<ServiceOptions> _serviceOptions;
        private readonly IMonitorHealthCheck _monitorHealthCheck;

        public ClientMonitorHealthCheck(
            IOptionsMonitor<MonitorOptions> monitorOptions,
            IOptionsMonitor<DatabaseOptions> databaseOptions,
            IOptionsMonitor<LoggerOptions> loggerOptions,
            IOptionsMonitor<ServiceOptions> serviceOptions,
            IMonitorHealthCheck monitorHealthCheck)
        {
            _monitorOptions = monitorOptions;
            _databaseOptions = databaseOptions;
            _loggerOptions = loggerOptions;
            _serviceOptions = serviceOptions;
            _monitorHealthCheck = monitorHealthCheck;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var pushHealthCheckModel = new HealthCheckRequest();
            if (_monitorOptions.CurrentValue.CheckDatabaseOption
                && _databaseOptions.CurrentValue != null)
            {
                switch (_databaseOptions.CurrentValue.ConnectionType)
                {
                    case ConnectionType.MongoDB:
                        try
                        {
                            var mongoClient = new MongoClient(_databaseOptions.CurrentValue.ConnectionString);
                            var mongoDatabase = mongoClient.GetDatabase(_databaseOptions.CurrentValue.Datasource);
                            var ping = await mongoDatabase.RunCommandAsync<BsonDocument>(new BsonDocument { { "ping", 1 } });
                            if (ping.Contains("ok") &&
                                    (ping["ok"].IsDouble && (int)ping["ok"].AsDouble == 1 ||
                                    ping["ok"].IsInt32 && ping["ok"].AsInt32 == 1))
                            {
                                pushHealthCheckModel.DatabaseHealthy = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            pushHealthCheckModel.DatabaseHealthy = false;
                            pushHealthCheckModel.ErrorStack = ex.ToString();
                        }
                        break;
                    case ConnectionType.SQLServer:
                        try
                        {
                            using (var sqlDbConnection = new SqlConnection(_databaseOptions.CurrentValue.ConnectionString))
                            {
                                sqlDbConnection.Open();
                                using (var sqlCommand = new SqlCommand("Select 1"))
                                {
                                    sqlCommand.ExecuteScalar();
                                    pushHealthCheckModel.DatabaseHealthy = true;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            pushHealthCheckModel.DatabaseHealthy = false;
                            pushHealthCheckModel.ErrorStack = ex.ToString();
                        }
                        break;
                    case ConnectionType.MySQL:
                        try
                        {
                            using (var mysqlDbConnection = new MySqlConnection(_databaseOptions.CurrentValue.ConnectionString))
                            {
                                mysqlDbConnection.Open();
                                using (var mysqlCommand = new MySqlCommand("Select 1"))
                                {
                                    mysqlCommand.ExecuteScalar();
                                    pushHealthCheckModel.DatabaseHealthy = true;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            pushHealthCheckModel.DatabaseHealthy = false;
                            pushHealthCheckModel.ErrorStack = ex.ToString();
                        }
                        break;
                    case ConnectionType.PostgreSQL:
                        try
                        {
                            using (var postgreDbConnection = new NpgsqlConnection(_databaseOptions.CurrentValue.ConnectionString))
                            {
                                postgreDbConnection.Open();
                                using (var postgreCommand = new NpgsqlCommand("Select 1"))
                                {
                                    postgreCommand.ExecuteScalar();
                                    pushHealthCheckModel.DatabaseHealthy = true;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            pushHealthCheckModel.DatabaseHealthy = false;
                            pushHealthCheckModel.ErrorStack = ex.ToString();
                        }
                        break;
                    default:
                        pushHealthCheckModel.DatabaseHealthy = false;
                        pushHealthCheckModel.ErrorStack = string.Format("A database type isn't supported in Healthcheck. Databse type: {0}", _databaseOptions.CurrentValue.ConnectionType.ToString());
                        break;
                }
            }
            else
            {
                pushHealthCheckModel.DatabaseHealthy = true;
            }

            _monitorHealthCheck.CalculateAvg();
            var hardwareCheck = _monitorHealthCheck.GetCurrentHardwareInfoHealthCheck();
            pushHealthCheckModel.HardwareInfoHealthCheck = new HardwareInfoHealthCheck();
            pushHealthCheckModel.HardwareInfoHealthCheck.CpuUsage = hardwareCheck.CpuUsage;
            pushHealthCheckModel.HardwareInfoHealthCheck.MemoryUsed = hardwareCheck.MemoryUsed;

            if (_monitorOptions.CurrentValue.CheckCpuBottleneck)
            {
                pushHealthCheckModel.HardwareInfoHealthCheck.IsCpuBottleNeck = pushHealthCheckModel.HardwareInfoHealthCheck.CpuUsage > _monitorOptions.CurrentValue.BottleneckCpu;
            }
            else
            {
                pushHealthCheckModel.HardwareInfoHealthCheck.IsCpuBottleNeck = false;
            }

            if (_monitorOptions.CurrentValue.CheckMemoryThreshold)
            {
                pushHealthCheckModel.HardwareInfoHealthCheck.IsMemoryThreshold = pushHealthCheckModel.HardwareInfoHealthCheck.MemoryUsed > _monitorOptions.CurrentValue.ThresholdMemory;
            }
            else
            {
                pushHealthCheckModel.HardwareInfoHealthCheck.IsMemoryThreshold = false;
            }

            // Collect more info from MonitorHealthcheck
            var currentHttpHealthCheck = _monitorHealthCheck.GetCurrentHttpHealthCheck();
            pushHealthCheckModel.HttpHealthCheck = new HttpHealthCheck
            {
                AvgDuration = currentHttpHealthCheck.AvgDuration,
                FailedRequests = currentHttpHealthCheck.FailedRequests,
                SuccessRequests = currentHttpHealthCheck.SuccessRequests,
                TotalRequestsPerDay = currentHttpHealthCheck.TotalRequestsPerDay
            };

            //pushHealthCheckModel.ServiceName = _serviceOptions.CurrentValue.Name;
            pushHealthCheckModel.Healthy = !pushHealthCheckModel.HardwareInfoHealthCheck.IsCpuBottleNeck
                                            && !pushHealthCheckModel.HardwareInfoHealthCheck.IsMemoryThreshold
                                            && pushHealthCheckModel.DatabaseHealthy;

            pushHealthCheckModel.BeatDate = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow);

            var data = new Dictionary<string, object>()
            {
                { Constants.CLIENT_MONITOR_HEALTHCHECK_DATA, pushHealthCheckModel }
            };

            if (pushHealthCheckModel.Healthy)
            {
                return new HealthCheckResult(HealthStatus.Healthy,
                    description:
                        string.Format("Healthy with checking statuses: CPU - {0}; Memory - {1}; Database - {2}",
                                GetHealthyWord(!pushHealthCheckModel.HardwareInfoHealthCheck.IsCpuBottleNeck),
                                GetHealthyWord(!pushHealthCheckModel.HardwareInfoHealthCheck.IsMemoryThreshold),
                                GetHealthyWord(pushHealthCheckModel.DatabaseHealthy)), data: data);
            }
            else
            {
                return new HealthCheckResult(HealthStatus.Unhealthy,
                    description:
                        string.Format("Unhealthy with checking statuses: CPU - {0}; Memory - {1}; Database - {2}",
                                GetHealthyWord(!pushHealthCheckModel.HardwareInfoHealthCheck.IsCpuBottleNeck),
                                GetHealthyWord(!pushHealthCheckModel.HardwareInfoHealthCheck.IsMemoryThreshold),
                                GetHealthyWord(pushHealthCheckModel.DatabaseHealthy)), data: data);
            }
        }

        private string GetHealthyWord(bool isHealthy)
        {
            return isHealthy ? "Healthy" : "Unhealthy";
        }
    }
}
