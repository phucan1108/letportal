using System;
using System.Threading.Tasks;
using LetPortal.Core.Monitors.Models;
using LetPortal.Core.Utils;
using LetPortal.ServiceManagement.Repositories.Abstractions;

namespace LetPortal.ServiceManagement.Providers
{
    public class MonitorProvider : IMonitorProvider
    {
        private readonly IMonitorCounterRepository _monitorCounterRepository;

        public MonitorProvider(IMonitorCounterRepository monitorCounterRepository)
        {
            _monitorCounterRepository = monitorCounterRepository;
        }

        public async Task AddMonitorPulse(PushHealthCheckModel pushHealthCheckModel)
        {
            await _monitorCounterRepository.AddAsync(new Entities.MonitorCounter
            {
                Id = DataUtil.GenerateUniqueId(),
                BeatDate = pushHealthCheckModel.BeatDate,
                ServiceId = pushHealthCheckModel.ServiceId,
                ServiceName = pushHealthCheckModel.ServiceName,
                HttpCounter = new Entities.HttpCounter
                {
                    AvgDuration = Math.Round(pushHealthCheckModel.HttpHealthCheck.AvgDuration, 0, MidpointRounding.AwayFromZero),
                    FailedRequests = pushHealthCheckModel.HttpHealthCheck.FailedRequests,
                    MeansureDate = pushHealthCheckModel.BeatDate,
                    SuccessRequests = pushHealthCheckModel.HttpHealthCheck.SuccessRequests,
                    TotalRequestsPerDay = pushHealthCheckModel.HttpHealthCheck.TotalRequestsPerDay,
                    ServiceId = pushHealthCheckModel.ServiceId
                },
                HardwareCounter = new Entities.HardwareCounter
                {
                    CpuUsage = Math.Round(pushHealthCheckModel.HardwareInfoHealthCheck.CpuUsage, 0, MidpointRounding.AwayFromZero),
                    MemoryUsed = pushHealthCheckModel.HardwareInfoHealthCheck.MemoryUsed,
                    MemoryUsedInMb = (int)Math.Round((double)(pushHealthCheckModel.HardwareInfoHealthCheck.MemoryUsed / 1024), 0, MidpointRounding.AwayFromZero),
                    IsCpuBottleneck = pushHealthCheckModel.HardwareInfoHealthCheck.IsCpuBottleneck,
                    IsMemoryThreshold = pushHealthCheckModel.HardwareInfoHealthCheck.IsMemoryThreshold,
                    MeansureDate = pushHealthCheckModel.BeatDate,
                    ServiceId = pushHealthCheckModel.ServiceId
                }
            });
        }
    }
}
