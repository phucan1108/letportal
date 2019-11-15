using LetPortal.Core.Persistences;
using LetPortal.ServiceManagement.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LetPortal.ServiceManagement.Repositories.Abstractions
{
    public interface IMonitorHardwareReportRepository : IGenericRepository<MonitorHardwareReport>
    {
        Task CollectDataAsync(DateTime reportDate, int duration, bool roundDate = true);
    }
}
