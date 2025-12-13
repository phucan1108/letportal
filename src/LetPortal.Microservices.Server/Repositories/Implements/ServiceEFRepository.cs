using System;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Microservices.Server.Entities;
using LetPortal.Microservices.Server.Repositories.Abstractions;

namespace LetPortal.Microservices.Server.Repositories.Implements
{
    public class ServiceEFRepository : EFGenericRepository<Service>, IServiceRepository
    {
        private readonly LetPortalServiceManagementDbContext _context;
        public ServiceEFRepository(LetPortalServiceManagementDbContext context)
            : base(context)
        {
            _context = context;
        }

        public Task ForceShutdownAllServices()
        {
            var allRunningServices = _context.Services.Where(a => a.ServiceState == ServiceState.Start || a.ServiceState == ServiceState.Run);
            foreach (var service in allRunningServices)
            {
                service.ServiceState = ServiceState.Shutdown;
            }
            _context.SaveChanges();
            return Task.CompletedTask;
        }

        public Task<int> GetLastInstanceNoOfService(string serviceName)
        {
            var allInstanceNos = _context.Services
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
            var allLosingServices = _context.Services
                                    .Where(a => (a.ServiceState == ServiceState.Run || a.ServiceState == ServiceState.Start) && a.LastCheckedDate <= lostDate).ToList();

            foreach (var service in allLosingServices)
            {
                service.ServiceState = ServiceState.Lost;
                service.LastCheckedDate = DateTime.UtcNow;
            }

            _context.SaveChanges();

            return Task.CompletedTask;
        }

        public Task UpdateShutdownStateForAllServices(int durationShutdown)
        {
            var lostDate = DateTime.UtcNow.AddSeconds(durationShutdown * -1);
            var allLosingServices = _context.Services
                                    .Where(a => a.ServiceState == ServiceState.Lost && a.LastCheckedDate <= lostDate).ToList();
            foreach (var service in allLosingServices)
            {
                service.ServiceState = ServiceState.Shutdown;
                service.LastCheckedDate = DateTime.UtcNow;
            }

            _context.SaveChanges();

            return Task.CompletedTask;
        }
    }
}
