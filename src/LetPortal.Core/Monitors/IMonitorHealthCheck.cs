using LetPortal.Core.Monitors.Models;

namespace LetPortal.Core.Monitors
{
    public interface IMonitorHealthCheck
    {
        void AddRequestMonitor(RequestMonitor requestMonitor);

        void CalculateAvg();

        HttpHealthCheckModel GetCurrentHttpHealthCheck();

        HardwareInfoHealthCheckModel GetCurrentHardwareInfoHealthCheck();

        void CleanUp();
    }
}
