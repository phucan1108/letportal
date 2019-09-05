using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LetPortal.Core.Utils
{
    public class DiagnosticUtil
    {
        public static async Task<double> GetCpuUsage()
        {
            var startTime = DateTime.UtcNow;
            var startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            var endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;
            var cpuUsedMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            await Task.Delay(100);
            var endTime = DateTime.UtcNow;
            var totalMsPassed = (endTime - startTime).TotalMilliseconds;
            var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);
            return cpuUsageTotal;
        }

        public static long GetMemoryUsedInKb()
        {
            return Process.GetCurrentProcess().WorkingSet64 / 1024;
        }
    }
}
