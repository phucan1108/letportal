using System.Threading.Tasks;
using LetPortal.Core.Monitors.Models;

namespace LetPortal.ServiceManagement.Providers
{
    public interface IMonitorProvider
    {
        Task AddMonitorPulse(PushHealthCheckModel pushHealthCheckModel);
    }
}
