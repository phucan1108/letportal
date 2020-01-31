using System.Threading.Tasks;

namespace LetPortal.ServiceManagement.Providers
{
    public interface IMonitorServiceReportProvider
    {
        Task CollectAndReportHardware(string[] serviceIds, int duration, bool roundMinute = true);

        Task CollectAndReportHttp(string[] serviceIds, int duration, bool roundMinute = true);
    }
}
