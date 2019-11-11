using LetPortal.Core.Persistences;
using LetPortal.ServiceManagement.Options;
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

            services.Configure<ServiceManagementOptions>(configuration.GetSection("ServiceManagementOptions"));

            services.AddSingleton(databaseOptions);
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

            services.AddTransient<IServiceManagementProvider, ServiceManagamentProvider>();
            services.AddTransient<ILogEventProvider, LogEventProvider>();
            services.AddTransient<IMonitorProvider, MonitorProvider>();

            services.AddHostedService<CheckingLostServicesBackgroundTask>();
            services.AddHostedService<CheckingShutdownServicesBackgroundTask>();
        }
    }
}
