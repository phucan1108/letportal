using System;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.ServiceManagement.Entities;

namespace LetPortal.ServiceManagement.Repositories.Abstractions
{
    public interface IMonitorHardwareReportRepository : IGenericRepository<MonitorHardwareReport>
    {
        Task CollectDataAsync(string[] collectServiceIds, DateTime reportDate, int duration, bool roundDate = true);
    }
}
