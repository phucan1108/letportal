using LetPortal.Core.Logger;
using LetPortal.Core.Monitors.Models;
using LetPortal.Core.Persistences;
using LetPortal.Core.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LetPortal.Core.Monitors
{
    public class LetPortalMonitorHealthCheck : IHealthCheck
    {
        private readonly IOptionsMonitor<MonitorOptions> _monitorOptions;
        private readonly IOptionsMonitor<DatabaseOptions> _databaseOptions;
        private readonly IOptionsMonitor<LoggerOptions> _loggerOptions;
        private readonly IOptionsMonitor<ServiceOptions> _serviceOptions;
        private readonly IMonitorHealthCheck _monitorHealthCheck;

        public LetPortalMonitorHealthCheck(
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
            var pushHealthCheckModel = new PushHealthCheckModel();
            if(_monitorOptions.CurrentValue.CheckDatabaseOption
                && _databaseOptions.CurrentValue != null)
            {
                switch(_databaseOptions.CurrentValue.ConnectionType)
                {
                    case ConnectionType.MongoDB:
                        try
                        {
                            var mongoClient = new MongoClient(_databaseOptions.CurrentValue.ConnectionString);
                            var mongoDatabase = mongoClient.GetDatabase(_databaseOptions.CurrentValue.Datasource);
                            var ping = await mongoDatabase.RunCommandAsync<BsonDocument>(new BsonDocument { { "ping", 1 } });
                            if(ping.Contains("ok") &&
                                    (ping["ok"].IsDouble && (int)ping["ok"].AsDouble == 1 ||
                                    ping["ok"].IsInt32 && ping["ok"].AsInt32 == 1))
                            {
                                pushHealthCheckModel.DatabaseHealthy = true;
                            }
                        }
                        catch(Exception ex)
                        {
                            pushHealthCheckModel.DatabaseHealthy = false;
                            pushHealthCheckModel.ErrorStack = ex.ToString();
                        }
                        break;
                    case ConnectionType.SQLServer:
                        pushHealthCheckModel.DatabaseHealthy = true;
                        break;
                    default:
                        pushHealthCheckModel.DatabaseHealthy = true;
                        break;
                }
            }
            else
            {
                pushHealthCheckModel.DatabaseHealthy = true;
            }

            if(_monitorOptions.CurrentValue.CheckDatabaseLoggerOption
                && _databaseOptions.CurrentValue != null)
            {
                pushHealthCheckModel.LoggerDatabaseHealthy = true;
            }

            _monitorHealthCheck.CalculateAvg();
            var hardwareCheck = _monitorHealthCheck.GetCurrentHardwareInfoHealthCheck();
            pushHealthCheckModel.HardwareInfoHealthCheck.CpuUsage = hardwareCheck.CpuUsage;
            pushHealthCheckModel.HardwareInfoHealthCheck.MemoryUsed = hardwareCheck.MemoryUsed;

            if(_monitorOptions.CurrentValue.CheckCpuBottleneck)
            {
                pushHealthCheckModel.HardwareInfoHealthCheck.IsCpuBottleneck = pushHealthCheckModel.HardwareInfoHealthCheck.CpuUsage > _monitorOptions.CurrentValue.BottleneckCpu;
            }
            else
            {
                pushHealthCheckModel.HardwareInfoHealthCheck.IsCpuBottleneck = false;
            }

            if(_monitorOptions.CurrentValue.CheckMemoryThreshold)
            {
                pushHealthCheckModel.HardwareInfoHealthCheck.IsMemoryThreshold = pushHealthCheckModel.HardwareInfoHealthCheck.MemoryUsed > _monitorOptions.CurrentValue.ThresholdMemory;
            }
            else
            {
                pushHealthCheckModel.HardwareInfoHealthCheck.IsMemoryThreshold = false;
            }

            // Collect more info from MonitorHealthcheck
            pushHealthCheckModel.HttpHealthCheck = _monitorHealthCheck.GetCurrentHttpHealthCheck();

            pushHealthCheckModel.ServiceName = _serviceOptions.CurrentValue.Name;
            pushHealthCheckModel.Healthy = !pushHealthCheckModel.HardwareInfoHealthCheck.IsCpuBottleneck
                                            && !pushHealthCheckModel.HardwareInfoHealthCheck.IsMemoryThreshold
                                            && pushHealthCheckModel.LoggerDatabaseHealthy
                                            && pushHealthCheckModel.DatabaseHealthy;

            pushHealthCheckModel.BeatDate = DateTime.UtcNow;

            var data = new Dictionary<string, object>()
            {
                {  Constants.LetPortalHealthCheckData, pushHealthCheckModel }
            };

            if(pushHealthCheckModel.Healthy)
            {
                return new HealthCheckResult(HealthStatus.Healthy, description: "Healthy", data: data);
            }
            else
            {
                return new HealthCheckResult(HealthStatus.Unhealthy, description: "Unhealthy", data: data);
            }
        }
    }
}
