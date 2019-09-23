using LetPortal.Core.Persistences;
using LetPortal.ServiceManagement.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace LetPortal.ServiceManagement.Repositories
{
    public class ServiceEFRepository : EFGenericRepository<Service>, IServiceRepository
    {
        private readonly LetPortalServiceManagementDbContext _context;
        public ServiceEFRepository(LetPortalServiceManagementDbContext context)
            : base(context)
        {
            _context = context;
        }

        public Task<int> GetLastInstanceNoOfService(string serviceName)
        {
            var allRegisteredServices = _context.Services.Where(a => (a.ServiceState != ServiceState.Shutdown && a.ServiceState != ServiceState.Lost) && a.Name == serviceName).ToList();

            if(allRegisteredServices != null && allRegisteredServices.Count > 0)
            {
                return Task.FromResult(allRegisteredServices.OrderByDescending(a => a.InstanceNo).First().InstanceNo);
            }

            return Task.FromResult(1);
        }

        public Task UpdateShutdownStateForAllServices()
        {
            var allRunningServices = _context.Services.Where(a => a.ServiceState == ServiceState.Start || a.ServiceState == ServiceState.Run);
            foreach(var service in allRunningServices)
            {
                service.ServiceState = ServiceState.Shutdown;
            }
            _context.SaveChanges();
            return Task.CompletedTask;
        }
    }
}
