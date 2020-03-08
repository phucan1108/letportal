using LetPortal.Core.Persistences;
using LetPortal.ServiceManagement.Entities;
using LetPortal.ServiceManagement.Repositories.Abstractions;

namespace LetPortal.ServiceManagement.Repositories.Implements
{
    public class MonitorCounterMongoRepository : MongoGenericRepository<MonitorCounter>, IMonitorCounterRepository
    {
        public MonitorCounterMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }
    }
}
