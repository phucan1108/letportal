using LetPortal.Core.Monitors.Models;
using LetPortal.Core.Utils;
using LetPortal.ServiceManagement.Repositories.Abstractions;
using System.Threading.Tasks;

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
                    AvgDuration = pushHealthCheckModel.HttpHealthCheck.AvgDuration,
                    FailedRequests = pushHealthCheckModel.HttpHealthCheck.FailedRequests,
                    MeansureDate = pushHealthCheckModel.HttpHealthCheck.MeansureDateTime,
                    SuccessRequests = pushHealthCheckModel.HttpHealthCheck.SuccessRequests,
                    TotalRequestsPerDay = pushHealthCheckModel.HttpHealthCheck.TotalRequestsPerDay,
                    ServiceId = pushHealthCheckModel.ServiceId
                },
                HardwareCounter = new Entities.HardwareCounter
                {
                    CpuUsage = pushHealthCheckModel.HardwareInfoHealthCheck.CpuUsage,
                    MemoryUsed = pushHealthCheckModel.HardwareInfoHealthCheck.MemoryUsed,
                    IsCpuBottleneck = pushHealthCheckModel.HardwareInfoHealthCheck.IsCpuBottleneck,
                    IsMemoryThreshold = pushHealthCheckModel.HardwareInfoHealthCheck.IsMemoryThreshold,
                    MeansureDate = pushHealthCheckModel.BeatDate,
                    ServiceId = pushHealthCheckModel.ServiceId
                }
            });
        }
    }
}
