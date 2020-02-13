using System;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.ServiceManagement.Entities;

namespace LetPortal.ServiceManagement.Repositories.Abstractions
{
    public interface IMonitorHttpReportRepository : IGenericRepository<MonitorHttpReport>
    {
        Task CollectDataAsync(string[] collectServiceIds, DateTime reportDate, int duration, bool roundDate = true);
    }
}
