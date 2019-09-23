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
            var databaseOptions = configuration.GetSection("DatabaseOptions").Get<DatabaseOptions>();
            if(databaseOptions.ConnectionType == ConnectionType.MongoDB)
            {

                ConventionPackDefault.Register();
                services.AddSingleton<MongoConnection>();
                services.AddSingleton<IServiceRepository, ServiceMongoRepository>();
                services.AddSingleton<ILogEventRepository, LogEventMongoRepository>();
                services.AddSingleton<IMonitorCounterRepository, MonitorCounterMongoRepository>();
            }

            if(databaseOptions.ConnectionType == ConnectionType.SQLServer
                || databaseOptions.ConnectionType == ConnectionType.PostgreSQL
                || databaseOptions.ConnectionType == ConnectionType.MySQL)
            {
                services.AddTransient<LetPortalServiceManagementDbContext>();
                services.AddTransient<IServiceRepository, ServiceEFRepository>();
                services.AddTransient<ILogEventRepository, LogEventEFRepository>();
                services.AddTransient<IMonitorCounterRepository, MonitorCounterEFRepository>();
            }

            services.AddSingleton<IServiceManagementProvider, ServiceManagamentProvider>();
            services.AddSingleton<ILogEventProvider, LogEventProvider>();
            services.AddSingleton<IMonitorProvider, MonitorProvider>();

        }
    }
}
