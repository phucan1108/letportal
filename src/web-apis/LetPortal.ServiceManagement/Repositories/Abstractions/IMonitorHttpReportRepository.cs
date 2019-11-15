using LetPortal.Core.Persistences;
using LetPortal.ServiceManagement.Entities;
using System;
using System.Threading.Tasks;

namespace LetPortal.ServiceManagement.Repositories.Abstractions
{
    public interface IMonitorHttpReportRepository : IGenericRepository<MonitorHttpReport>
    {
        Task CollectDataAsync(DateTime reportDate, int duration, bool roundDate = true);
    }
}
