using LetPortal.Core.Persistences;
using LetPortal.ServiceManagement.Entities;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace LetPortal.ServiceManagement.Repositories
{
    public class ServiceMongoRepository : MongoGenericRepository<Service>, IServiceRepository
    {
        public ServiceMongoRepository(MongoConnection mongoConnection)
        {
            Connection = mongoConnection;
        }

        public async Task ForceShutdownAllServices()
        {
            var updateBuilder = Builders<Service>.Update.Set(a => a.ServiceState, ServiceState.Shutdown);
            await Collection.UpdateManyAsync(a => a.ServiceState == ServiceState.Start || a.ServiceState == ServiceState.Run, updateBuilder);
        }

        public async Task<int> GetLastInstanceNoOfService(string serviceName)
        {
            var allRegisteredServices = await Collection.AsQueryable().Where(a => (a.ServiceState != ServiceState.Shutdown && a.ServiceState != ServiceState.Lost) && a.Name == serviceName).ToListAsync();

            if (allRegisteredServices != null && allRegisteredServices.Count > 0)
            {
                return allRegisteredServices.OrderByDescending(a => a.InstanceNo).First().InstanceNo;
            }

            return 1;
        }

        public Task UpdateLostStateForAllLosingServices(int durationLost)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateShutdownStateForAllServices(int durationShutdown)
        {
            throw new System.NotImplementedException();
        }
    }
}
