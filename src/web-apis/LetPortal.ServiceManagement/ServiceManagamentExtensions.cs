using LetPortal.Core.Persistences;
using LetPortal.ServiceManagement.Providers;
using LetPortal.ServiceManagement.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LetPortal.ServiceManagement
{
    public static class ServiceManagamentExtensions
    {
        public static void AddServiceManagement(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DatabaseOptions>(configuration.GetSection("DatabaseOptions"));
            services.AddSingleton<MongoConnection>();
            services.AddSingleton<IServiceRepository, ServiceMongoRepository>();
            services.AddSingleton<ILogEventRepository, LogEventMongoRepository>();
            services.AddSingleton<IMonitorCounterRepository, MonitorCounterMongoRepository>();
            services.AddSingleton<IServiceManagementProvider, ServiceManagamentProvider>();
            services.AddSingleton<ILogEventProvider, LogEventProvider>();
            services.AddSingleton<IMonitorProvider, MonitorProvider>();            
        }
    }
}
