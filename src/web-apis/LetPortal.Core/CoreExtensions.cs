using LetPortal.Core.Configurations;
using LetPortal.Core.Exceptions;
using LetPortal.Core.Logger;
using LetPortal.Core.Logger.Repositories;
using LetPortal.Core.Monitors;
using LetPortal.Core.Persistences;
using LetPortal.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using System;
using System.IO;

namespace LetPortal.Core
{
    public static class CoreExtensions
    {
        public static ILetPortalBuilder AddLetPortal(this IServiceCollection serviceCollection, IConfiguration configuration, Action<LetPortalOptions> action = null)
        {
            var letPortalOptions = new LetPortalOptions();
            if(action != null)
            {
                action.Invoke(letPortalOptions);
            }

            var builder = new LetPortalBuilder(serviceCollection, configuration, letPortalOptions);

            if(letPortalOptions.EnableDatabaseConnection)
            {
                builder.Services.Configure<DatabaseOptions>(configuration.GetSection("DatabaseOptions"));
                var databaseOptions = configuration.GetSection("DatabaseOptions").Get<DatabaseOptions>();
                builder.SetConnectionType(databaseOptions.ConnectionType);
                if(databaseOptions.ConnectionType == ConnectionType.MongoDB)
                {
                    ConventionPackDefault.Register();
                    builder.Services.AddSingleton<MongoConnection>();
                }
            }

            if(letPortalOptions.EnableMicroservices)
            {
                builder.Services.Configure<ServiceOptions>(configuration.GetSection("ServiceOptions"));
                builder.Services.AddHttpClient<IServiceContext, ServiceContext>();
            }

            if(letPortalOptions.EnableServiceMonitor)
            {
                builder.Services.Configure<MonitorOptions>(configuration.GetSection("MonitorOptions"));
                builder.Services.AddSingleton<IMonitorHealthCheck, MonitorHealthCheck>();
                var monitorOption = configuration.GetSection("MonitorOptions").Get<MonitorOptions>();
                if(monitorOption.Enable)
                {
                    if(monitorOption.NotifyOptions.Enable)
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

            if(letPortalOptions.EnableSerilog)
            {
                builder.Services.Configure<LoggerOptions>(configuration.GetSection("LoggerOptions"));
                var serviceOptions = configuration.GetSection("ServiceOptions").Get<ServiceOptions>();
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .Enrich.WithProperty("ServiceName", serviceOptions.Name)
                    .CreateLogger();

                builder.Services.AddSingleton(Log.Logger);
                builder.Services.AddTransient(typeof(IServiceLogger<>), typeof(ServiceLogger<>));
                builder.Services.AddSingleton<ILogRepository, LogMongoRepository>();
            }

            return builder;
        }

        public static void UseLetPortal(
            this IApplicationBuilder app,
            IApplicationLifetime applicationLifetime,
            Action<LetPortalMiddlewareOptions> options = null)
        {
            var defaultOptions = new LetPortalMiddlewareOptions();
            if(options != null)
            {
                options.Invoke(defaultOptions);
            }

            applicationLifetime.RegisterServiceLifecycle(app);

            if(defaultOptions.EnableCheckUserSession)
            {
                if(defaultOptions.SkipCheckUrls != null && defaultOptions.SkipCheckUrls.Length > 0)
                {
                    app.UseWhen(context =>
                    {
                        bool isSkipped = false;
                        foreach(var url in defaultOptions.SkipCheckUrls)
                        {
                            isSkipped = context.Request.Path.ToString().Contains(url);
                            if(isSkipped)
                            {
                                break;
                            }
                        }

                        return !isSkipped;

                    }, builder =>
                    {
                        builder.UseMiddleware<CheckUserSessionIdMiddleware>();
                        builder.UseMiddleware<CheckTraceIdMiddleware>();
                    });
                }
                else
                {
                    app.UseMiddleware<CheckUserSessionIdMiddleware>();
                    app.UseMiddleware<CheckTraceIdMiddleware>();
                }
                
            }

            if(defaultOptions.EnableCheckUserSession 
                && defaultOptions.EnableWrapException)
            {
                app.UseMiddleware<NotifyExceptionLogMiddleware>();
                app.UseMiddleware<AddRequestMonitorMiddleware>();
            }


            app.UseMiddleware<CatchGlobalExceptionMiddleware>();
        }

        /// <summary>
        /// Allow service to load configuration from Configuration Service by servicename and version
        /// Note: If you don't have any idea to use configuration service, don't call it
        /// </summary>
        public static IConfigurationBuilder AddConfigurationService(this IConfigurationBuilder builder)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration  = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var configurationServiceOptions = configuration.GetSection("ConfigurationServiceOptions").Get<ConfigurationServiceOptions>();
            var serviceOptions = configuration.GetSection("ServiceOptions").Get<ServiceOptions>();

            if(configurationServiceOptions == null || serviceOptions == null)
            {
                throw new Exception("Missing ConfiguartionServiceOptions or ServiceOptions in appsettings.json, please check again!");
            }

            return builder.Add(new IntegratorConfigurationServiceSource(configurationServiceOptions, serviceOptions.Name, serviceOptions.Version));
        }



        /// <summary>
        /// Notify to Service Management when service is starting or stopping
        /// </summary>
        private static void RegisterServiceLifecycle(this Microsoft.AspNetCore.Hosting.IApplicationLifetime applicationLifetime, IApplicationBuilder applicationBuilder, Action postStartAction = null, Action postStopAction = null)
        {
            var serviceContext = applicationBuilder.ApplicationServices.GetService<IServiceContext>();
            if(serviceContext != null)
            {
                applicationLifetime.ApplicationStarted.Register(() =>
                {
                    serviceContext.Start(postStartAction);
                });

                applicationLifetime.ApplicationStopping.Register(() =>
                {
                    serviceContext.Stop(postStopAction);
                });
            }
        }

    }
}
