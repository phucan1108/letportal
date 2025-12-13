using System;
using System.Threading.Tasks;
using LetPortal.Core.Monitors.Models;
using LetPortal.Core.Utils;
using LetPortal.Microservices.Monitors;
using LetPortal.Microservices.Server.Entities;
using LetPortal.Microservices.Server.Repositories.Abstractions;

namespace LetPortal.Microservices.Server.Providers
{
    public class MonitorProvider : IMonitorProvider
    {
        private readonly IMonitorCounterRepository _monitorCounterRepository;

        public MonitorProvider(IMonitorCounterRepository monitorCounterRepository)
        {
            _monitorCounterRepository = monitorCounterRepository;
        }

        public async Task AddMonitorPulse(HealthCheckRequest healthCheckRequest)
        {
            await _monitorCounterRepository.AddAsync(new MonitorCounter
            {
                Id = DataUtil.GenerateUniqueId(),
                BeatDate = healthCheckRequest.BeatDate.ToDateTime(),
                ServiceId = healthCheckRequest.ServiceId,
                ServiceName = healthCheckRequest.ServiceName,
                HttpCounter = new HttpCounter
                {
                    AvgDuration = Math.Round(healthCheckRequest.HttpHealthCheck.AvgDuration, 0, MidpointRounding.AwayFromZero),
                    FailedRequests = healthCheckRequest.HttpHealthCheck.FailedRequests,
                    MeansureDate = healthCheckRequest.BeatDate.ToDateTime(),
                    SuccessRequests = healthCheckRequest.HttpHealthCheck.SuccessRequests,
                    TotalRequestsPerDay = healthCheckRequest.HttpHealthCheck.TotalRequestsPerDay,
                    ServiceId = healthCheckRequest.ServiceId
                },
                HardwareCounter = new HardwareCounter
                {
                    CpuUsage = Math.Round(healthCheckRequest.HardwareInfoHealthCheck.CpuUsage, 0, MidpointRounding.AwayFromZero),
                    MemoryUsed = healthCheckRequest.HardwareInfoHealthCheck.MemoryUsed,
                    MemoryUsedInMb = (int)Math.Round((double)(healthCheckRequest.HardwareInfoHealthCheck.MemoryUsed / 1024), 0, MidpointRounding.AwayFromZero),
                    IsCpuBottleneck = healthCheckRequest.HardwareInfoHealthCheck.IsCpuBottleNeck,
                    IsMemoryThreshold = healthCheckRequest.HardwareInfoHealthCheck.IsMemoryThreshold,
                    MeansureDate = healthCheckRequest.BeatDate.ToDateTime(),
                    ServiceId = healthCheckRequest.ServiceId
                }
            });
        }
    }
}
