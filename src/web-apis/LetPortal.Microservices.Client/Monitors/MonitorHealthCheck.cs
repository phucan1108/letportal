using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LetPortal.Core.Monitors;
using LetPortal.Core.Monitors.Models;
using LetPortal.Core.Utils;
using Microsoft.Extensions.Options;

namespace LetPortal.Microservices.Client.Monitors
{
    public class MonitorHealthCheck : IMonitorHealthCheck, IDisposable
    {
        private readonly IOptionsMonitor<MonitorOptions> _monitorOptions;

        private HttpHealthCheckModel currentHttpHealthCheck = new HttpHealthCheckModel();

        private HardwareInfoHealthCheckModel currentHardwareInfoHealthCheck = new HardwareInfoHealthCheckModel();

        private List<RequestMonitor> requestMonitors = new List<RequestMonitor>();

        object sessionLock = new object();

        public MonitorHealthCheck(IOptionsMonitor<MonitorOptions> monitorOptions)
        {
            _monitorOptions = monitorOptions;
        }

        public void AddRequestMonitor(RequestMonitor requestMonitor)
        {
            requestMonitors.Add(requestMonitor);
        }

        public HttpHealthCheckModel GetCurrentHttpHealthCheck()
        {
            Monitor.Enter(sessionLock);
            try
            {
                return currentHttpHealthCheck;
            }
            finally
            {
                Monitor.Exit(sessionLock);
            }
        }

        private void CalculateAvgHealthCheck()
        {
            if (DateUtil.GetDateByTz(currentHttpHealthCheck.MeansureDateTime, _monitorOptions.CurrentValue.ResetMonitorTimezone).Day < DateUtil.GetCurrentSystemDateByTz(_monitorOptions.CurrentValue.ResetMonitorTimezone).Day)
            {
                // Reset monitor counter
                currentHttpHealthCheck = new HttpHealthCheckModel();
                currentHardwareInfoHealthCheck = new HardwareInfoHealthCheckModel();
            }
            var groupedByToday = requestMonitors
                .Where(a => DateUtil.GetDateByTz(a.EndDateTime, _monitorOptions.CurrentValue.ResetMonitorTimezone).Day
                    == DateUtil.GetCurrentSystemDateByTz(_monitorOptions.CurrentValue.ResetMonitorTimezone).Day);
            currentHttpHealthCheck.TotalRequestsPerDay += groupedByToday.Count();
            if (currentHttpHealthCheck.TotalRequestsPerDay == 0)
            {
                currentHttpHealthCheck.SuccessRequests = 0;
                currentHttpHealthCheck.FailedRequests = 0;
                currentHttpHealthCheck.AvgDuration = 0;
                currentHttpHealthCheck.MeansureDateTime = DateTime.UtcNow;

                currentHardwareInfoHealthCheck.CpuUsage = DiagnosticUtil.GetCpuUsage().Result;
                currentHardwareInfoHealthCheck.MemoryUsed = DiagnosticUtil.GetMemoryUsedInKb();
            }
            else
            {
                if (groupedByToday.Count() > 0)
                {
                    // Only count status code != 500 Internal Server Error
                    currentHttpHealthCheck.SuccessRequests += groupedByToday.Where(a => a.StatusCode == 200 || a.StatusCode == 204).Count();
                    currentHttpHealthCheck.FailedRequests += groupedByToday.Where(a => a.StatusCode != 200 && a.StatusCode != 204).Count();
                    var avgDuration = groupedByToday.Sum(b => b.ElapsedTime) / groupedByToday.Count();
                    currentHttpHealthCheck.AvgDuration = (currentHttpHealthCheck.AvgDuration + avgDuration) / 2;
                    currentHttpHealthCheck.MeansureDateTime = DateTime.UtcNow;

                    var avgCpuUsage = groupedByToday.Sum(b => b.CpuUsage) / groupedByToday.Count();
                    currentHardwareInfoHealthCheck.CpuUsage = avgCpuUsage;
                    var avgMemoryUsed = groupedByToday.Sum(b => b.MemoryUsed) / groupedByToday.Count();
                    currentHardwareInfoHealthCheck.MemoryUsed = avgMemoryUsed;

                    requestMonitors.Clear();
                }
                else
                {
                    currentHardwareInfoHealthCheck.CpuUsage = DiagnosticUtil.GetCpuUsage().Result;
                    currentHardwareInfoHealthCheck.MemoryUsed = DiagnosticUtil.GetMemoryUsedInKb();
                }
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(currentHttpHealthCheck);
            GC.SuppressFinalize(requestMonitors);
        }

        public HardwareInfoHealthCheckModel GetCurrentHardwareInfoHealthCheck()
        {
            Monitor.Enter(sessionLock);
            try
            {
                return currentHardwareInfoHealthCheck;
            }
            finally
            {
                Monitor.Exit(sessionLock);
            }
        }

        public void CalculateAvg()
        {
            Monitor.Enter(sessionLock);
            try
            {
                CalculateAvgHealthCheck();
            }
            finally
            {
                Monitor.Exit(sessionLock);
            }
        }

        public void CleanUp()
        {
            Monitor.Enter(sessionLock);
            try
            {
                requestMonitors.Clear();
            }
            finally
            {
                Monitor.Exit(sessionLock);
            }
        }
    }
}
