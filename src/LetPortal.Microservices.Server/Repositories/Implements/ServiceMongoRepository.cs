using System;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Microservices.Server.Entities;
using LetPortal.Microservices.Server.Repositories.Abstractions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace LetPortal.Microservices.Server.Repositories.Implements
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

        public Task<int> GetLastInstanceNoOfService(string serviceName)
        {
            var allInstanceNos = Collection.AsQueryable()
                    .Where(a => a.ServiceState != ServiceState.Shutdown && a.Name == serviceName)
                    .OrderByDescending(a => a.InstanceNo).Select(b => b.InstanceNo).ToList();

            if (allInstanceNos != null && allInstanceNos.Count > 0)
            {
                var counter = 1;
                var temp = allInstanceNos.OrderBy(a => a);
                foreach (var no in temp)
                {
                    if (counter < no)
                    {
                        // This service no has been terminated or lost
                        break;
                    }
                    counter++;
                }

                return Task.FromResult(counter);
            }

            return Task.FromResult(1);
        }

        public Task UpdateLostStateForAllLosingServices(int durationLost)
        {
            var lostDate = DateTime.UtcNow.AddSeconds(durationLost * -1);

            var updateFilterBuilder = Builders<Service>.Filter;
            var updateFilter = updateFilterBuilder.Where(a => (a.ServiceState == ServiceState.Run || a.ServiceState == ServiceState.Start) && a.LastCheckedDate <= lostDate);

            var updateDefBuider = Builders<Service>.Update;
            var updateDef = updateDefBuider.Set(a => a.ServiceState, ServiceState.Lost).Set(b => b.LastCheckedDate, DateTime.UtcNow);
            Collection.UpdateMany(updateFilter, updateDef);

            return Task.CompletedTask;
        }

        public Task UpdateShutdownStateForAllServices(int durationShutdown)
        {
            var shutdownDate = DateTime.UtcNow.AddSeconds(durationShutdown * -1);

            var updateFilterBuilder = Builders<Service>.Filter;
            var updateFilter = updateFilterBuilder.Where(a => a.ServiceState == ServiceState.Lost && a.LastCheckedDate <= shutdownDate);

            var updateDefBuider = Builders<Service>.Update;
            var updateDef = updateDefBuider.Set(a => a.ServiceState, ServiceState.Shutdown).Set(b => b.LastCheckedDate, DateTime.UtcNow);
            Collection.UpdateMany(updateFilter, updateDef);

            return Task.CompletedTask;
        }
    }
}
