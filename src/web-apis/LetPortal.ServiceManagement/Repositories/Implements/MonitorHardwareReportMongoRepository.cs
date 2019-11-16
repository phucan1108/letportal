using System;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.ServiceManagement.Entities;
using LetPortal.ServiceManagement.Repositories.Abstractions;

namespace LetPortal.ServiceManagement.Repositories.Implements
{
    public class MonitorHardwareReportMongoRepository : MongoGenericRepository<MonitorHardwareReport>, IMonitorHardwareReportRepository
    {
        public MonitorHardwareReportMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }

        public Task CollectDataAsync(string[] collectServiceIds, DateTime reportDate, int duration, bool roundDate = true)
        {
            throw new NotImplementedException();
        }
    }
}
