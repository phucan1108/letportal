using LetPortal.Core.Persistences;
using LetPortal.ServiceManagement.Entities;

namespace LetPortal.ServiceManagement.Repositories
{
    public class MonitorCounterMongoRepository : MongoGenericRepository<MonitorCounter>, IMonitorCounterRepository
    {
        public MonitorCounterMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }
    }
}
