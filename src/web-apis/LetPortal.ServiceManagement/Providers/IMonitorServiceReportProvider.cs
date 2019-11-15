using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LetPortal.ServiceManagement.Providers
{
    public interface IMonitorServiceReportProvider
    {
        Task CollectAndReportHardware(int duration, bool roundMinute = true);

        Task CollectAndReportHttp(int duration, bool roundMinute = true);
    }
}
