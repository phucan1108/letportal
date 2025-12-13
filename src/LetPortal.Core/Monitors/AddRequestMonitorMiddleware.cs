using System;
using System.Diagnostics;
using System.Threading.Tasks;
using LetPortal.Core.Extensions;
using LetPortal.Core.Logger;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace LetPortal.Core.Monitors
{
    public class AddRequestMonitorMiddleware
    {
        private readonly RequestDelegate _next;

        public AddRequestMonitorMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(
            HttpContext httpContext,
            IOptionsMonitor<MonitorOptions> monitorOption,
            IMonitorHealthCheck monitorHealthCheck,
            IServiceLogger<AddRequestMonitorMiddleware> serviceLogger)
        {
            if (monitorOption.CurrentValue.Enable)
            {
                var requestMonitor = new RequestMonitor
                {
                    BeginDateTime = DateTime.UtcNow
                };
                var startTime = DateTime.UtcNow;
                var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                await _next.Invoke(httpContext);
                stopWatch.Stop();
                var endTime = DateTime.UtcNow;
                var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
                requestMonitor.ElapsedTime = stopWatch.ElapsedMilliseconds;
                requestMonitor.EndDateTime = DateTime.UtcNow;
                var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
                var totalMsPassed = (endTime - startTime).TotalMilliseconds;
                var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);
                requestMonitor.CpuUsage = cpuUsageTotal * 100;
                if(requestMonitor.CpuUsage > 100)
                {
                    requestMonitor.CpuUsage = 100;
                }
                requestMonitor.StatusCode = httpContext.Response.StatusCode;
                requestMonitor.MemoryUsed = Process.GetCurrentProcess().WorkingSet64 / 1024;
                monitorHealthCheck.AddRequestMonitor(requestMonitor);
                httpContext.Items[Constants.RequestMonitor] = requestMonitor;
                serviceLogger.Info("Request complete with these usages {$Monitor}", requestMonitor.ToJson());
            }
            else
            {
                await _next.Invoke(httpContext);
            }
        }
    }
}
