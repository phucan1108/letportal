using System;
using System.Runtime.CompilerServices;
using LetPortal.Core;
using LetPortal.Core.Logger;
using LetPortal.Core.Monitors;
using LetPortal.Core.Persistences;
using LetPortal.Core.Services;
using LetPortal.ServiceManagement.Options;
using LetPortal.ServiceManagement.Providers;
using LetPortal.ServiceManagement.Repositories;
using LetPortal.ServiceManagement.Repositories.Abstractions;
using LetPortal.ServiceManagement.Repositories.Implements;
using LetPortal.ServiceManagement.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace LetPortal.ServiceManagement
{
    public static class ServiceManagamentExtensions
    {

        public static ILetPortalBuilder ActAsServiceManagement(
                this IServiceCollection serviceCollection,
                IConfiguration configuration,
                Action<LetPortalOptions> action = null)
        {
            var serviceOptions = configuration.GetSection("ServiceOptions").Get<ServiceOptions>();
            var letPortalOptions = new LetPortalOptions();
            if (action != null)
            {
                action.Invoke(letPortalOptions);
            }
            var corsOptions = configuration.GetSection("CorsPortalOptions").Get<Core.Configurations.CorsPortalOptions>();
            
            var builder = new LetPortalBuilder(
                serviceCollection,
                configuration,
                letPortalOptions,
                corsOptions,
                serviceOptions);

            if (letPortalOptions.EnableDatabaseConnection)
            {
                builder.Services.Configure<DatabaseOptions>(configuration.GetSection("DatabaseOptions"));
                var databaseOptions = configuration.GetSection("DatabaseOptions").Get<DatabaseOptions>();
                builder.Services.AddSingleton(databaseOptions);
                builder.SetConnectionType(databaseOptions.ConnectionType);
                if (databaseOptions.ConnectionType == ConnectionType.MongoDB)
                {
                    ConventionPackDefault.Register();
                    builder.Services.AddSingleton<MongoConnection>();
                }
            }

            if (letPortalOptions.EnableMicroservices)
            {
                builder.Services.Configure<ServiceOptions>(configuration.GetSection("ServiceOptions"));
                builder.Services.AddSingleton<IServiceContext, InternalServiceContext>();
            }

            if (letPortalOptions.EnableServiceMonitor)
            {
                builder.Services.Configure<MonitorOptions>(configuration.GetSection("MonitorOptions"));
                builder.Services.AddSingleton<IMonitorHealthCheck, MonitorHealthCheck>();
                var monitorOption = configuration.GetSection("MonitorOptions").Get<MonitorOptions>();
                if (monitorOption.Enable)
                {
                    if (monitorOption.NotifyOptions.Enable)
                    {
                        builder.Services.Configure<HealthCheckPublisherOptions>(opts =>
                        {
                            opts.Delay = TimeSpan.FromSeconds(monitorOption.NotifyOptions.Delay);
                        });

                        builder.Services.AddSingleton<IHealthCheckPublisher, LetPortalMonitorHealthCheckPublisher>();
                    }
                    builder.SetHealthCheckBuilder(builder.Services.AddHealthChecks().AddCheck<LetPortalMonitorHealthCheck>(Constants.LetPortalHealthCheck));
                }
            }

            if (letPortalOptions.EnableSerilog)
            {
                builder.Services.Configure<LoggerOptions>(configuration.GetSection("LoggerOptions"));
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .Enrich.WithProperty("ServiceName", serviceOptions.Name)
                    .CreateLogger();

                builder.Services.AddSingleton(Log.Logger);
                builder.Services.AddTransient(typeof(IServiceLogger<>), typeof(ServiceLogger<>));
            }

            return builder;
        }

        public static ILetPortalBuilder AddServiceManagement(this ILetPortalBuilder builder)
        {
            var databaseOptions = builder.Configuration.GetSection("DatabaseOptions").Get<DatabaseOptions>();

            builder.Services.Configure<ServiceManagementOptions>(builder.Configuration.GetSection("ServiceManagementOptions"));
            builder.Services.Configure<CentralizedLogOptions>(builder.Configuration.GetSection("CentralizedLogOptions"));
            if (databaseOptions.ConnectionType == ConnectionType.MongoDB)
            {
                builder.Services.AddSingleton<IServiceRepository, ServiceMongoRepository>();
                builder.Services.AddSingleton<ILogEventRepository, LogEventMongoRepository>();
                builder.Services.AddSingleton<IMonitorCounterRepository, MonitorCounterMongoRepository>();
                builder.Services.AddSingleton<IMonitorHardwareReportRepository, MonitorHardwareReportMongoRepository>();
                builder.Services.AddSingleton<IMonitorHttpReportRepository, MonitorHttpReportMongoRepository>();
            }

            if (databaseOptions.ConnectionType == ConnectionType.SQLServer
                || databaseOptions.ConnectionType == ConnectionType.PostgreSQL
                || databaseOptions.ConnectionType == ConnectionType.MySQL)
            {
                builder.Services.AddTransient<LetPortalServiceManagementDbContext>();
                builder.Services.AddTransient<IServiceRepository, ServiceEFRepository>();
                builder.Services.AddTransient<ILogEventRepository, LogEventEFRepository>();
                builder.Services.AddTransient<IMonitorCounterRepository, MonitorCounterEFRepository>();
                builder.Services.AddTransient<IMonitorHardwareReportRepository, MonitorHardwareReportEFRepository>();
                builder.Services.AddTransient<IMonitorHttpReportRepository, MonitorHttpReportEFRepository>();
            }

            builder.Services.AddSingleton<IServiceManagementProvider, ServiceManagamentProvider>();
            builder.Services.AddSingleton<IMonitorServiceReportProvider, MonitorServiceReportProvider>();
            builder.Services.AddSingleton<ILogEventProvider, LogEventProvider>();
            builder.Services.AddSingleton<IMonitorProvider, MonitorProvider>();

            builder.Services.AddHostedService<CheckingLostServicesBackgroundTask>();
            builder.Services.AddHostedService<CheckingShutdownServicesBackgroundTask>();
            builder.Services.AddHostedService<GatherAllHardwareCounterServicesBackgroundTask>();

            return builder;
        }

        public static void UseSelfMonitor(this IApplicationBuilder app, IHostApplicationLifetime hostLifeTime)
        {
            hostLifeTime.RegisterServiceLifecycle(app);
        }

        private static void RegisterServiceLifecycle(this IHostApplicationLifetime applicationLifetime,
            IApplicationBuilder applicationBuilder,
            Action postStartAction = null,
            Action postStopAction = null)
        {
            var serviceContext = applicationBuilder.ApplicationServices.GetService<IServiceContext>();
            var serviceManagementProvider = applicationBuilder.ApplicationServices.GetService<IServiceManagementProvider>();
            if (serviceContext != null)
            {
                applicationLifetime.ApplicationStarted.Register(() =>
                {
                    serviceContext.Start(postStartAction);
                    serviceContext.Run(postStartAction);
                });

                applicationLifetime.ApplicationStopping.Register(() =>
                {
                    serviceContext.Stop(postStopAction);
                    serviceManagementProvider.ShutdownAllServices().Wait();
                });
            }
        }
    }
}
