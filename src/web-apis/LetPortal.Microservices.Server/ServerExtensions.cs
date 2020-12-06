using System;
using LetPortal.Core;
using LetPortal.Core.Exceptions;
using LetPortal.Core.Logger;
using LetPortal.Core.Microservices.Configurations.Server;
using LetPortal.Core.Monitors;
using LetPortal.Core.Persistences;
using LetPortal.Microservices.Server.ConfigurationProviders;
using LetPortal.Microservices.Server.Middlewares;
using LetPortal.Microservices.Server.Monitors;
using LetPortal.Microservices.Server.Options;
using LetPortal.Microservices.Server.Providers;
using LetPortal.Microservices.Server.Repositories;
using LetPortal.Microservices.Server.Repositories.Abstractions;
using LetPortal.Microservices.Server.Repositories.Implements;
using LetPortal.Microservices.Server.Services;
using LetPortal.Microservices.Server.Workers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace LetPortal.Microservices.Server
{
    public static class ServerExtensions
    {
        private static string SERVER_CORS = "SATURN_SERVER_CORS";

        public static IConfigurationBuilder AddServicePerDirectory(this IConfigurationBuilder builder,
            string directoryPath,
            string environment,
            string sharedFolder,
            string ignoreCombineSharedServices)
        {
            return builder.Add(new ServicePerDirectoryConfigurationSource(directoryPath, environment, sharedFolder, ignoreCombineSharedServices));
        }

        public static IServiceCollection ActAsServer(
                this IServiceCollection services,
                IConfiguration configuration,
                Action<SelfServerOptions> action = null)
        {
            var selfServerOptions = new SelfServerOptions();
            if(action != null)
            {
                action.Invoke(selfServerOptions);
            }

            if (selfServerOptions.EnableDatabaseOptions)
            {
                services.Configure<DatabaseOptions>(configuration.GetSection("DatabaseOptions"));
            }

            if (selfServerOptions.EnableServiceMonitor)
            {
                services.Configure<MonitorOptions>(configuration.GetSection("MonitorOptions"));
                var monitorOptions = configuration.GetSection("MonitorOptions").Get<MonitorOptions>();
                services.AddSingleton<IMonitorHealthCheck, MonitorHealthCheck>();
                if (monitorOptions.Enable)
                {
                    if (monitorOptions.NotifyOptions.Enable)
                    {
                        services.Configure<Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckPublisherOptions>(opts =>
                        {
                            opts.Delay = TimeSpan.FromSeconds(monitorOptions.NotifyOptions.Delay);
                        });

                        services.AddSingleton<IHealthCheckPublisher, ServerMonitorHealthCheckPublisher>();
                    }
                    services.AddHealthChecks().AddCheck<ServerMonitorHealthCheck>(Constants.SERVER_MONITOR_HEALTHCHECK);
                }
            } 

            if (selfServerOptions.EnableSerilog)
            {
                services.Configure<LoggerOptions>(configuration.GetSection("LoggerOptions"));
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .Enrich.WithProperty("ServiceName", selfServerOptions.ServerName)
                    .CreateLogger();

                services.AddSingleton(Log.Logger);
                services.AddTransient(typeof(IServiceLogger<>), typeof(ServiceLogger<>));
            }

            if (selfServerOptions.EnableBuiltInCors)
            {
                var corsOptions = configuration.GetSection("CorsPortalOptions").Get<Core.Configurations.CorsPortalOptions>();
                services.AddCors(
                option =>
                {
                    option.AddPolicy(SERVER_CORS, corsBuilder =>
                    {
                        if (corsOptions.AllowAny)
                        {
                            corsBuilder.AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowAnyOrigin()
                               .WithExposedHeaders(corsOptions.ExposedHeaders.ToArray());
                        }
                        else
                        {
                            if (corsOptions.AllowCredentials)
                            {
                                corsBuilder
                                .AllowCredentials();
                            }

                            if (corsOptions.AllowAnyHeader)
                            {
                                corsBuilder.AllowAnyHeader();
                            }
                            else
                            {
                                corsBuilder.WithHeaders(corsOptions.AllowedHeaders.ToArray());
                            }

                            if (corsOptions.AllowAnyMethod)
                            {
                                corsBuilder.AllowAnyMethod();
                            }
                            else
                            {
                                corsBuilder.WithMethods(corsOptions.AllowedMethods.ToArray());
                            }

                            if (corsOptions.AllowAnyHost && !corsOptions.AllowCredentials)
                            {
                                corsBuilder.AllowAnyOrigin();
                            }
                            else
                            {
                                corsBuilder.WithOrigins(corsOptions.AllowedHosts.ToArray());
                            }

                            if (corsOptions.ExposedHeaders != null)
                            {
                                corsBuilder.WithExposedHeaders(corsOptions.ExposedHeaders.ToArray());
                            }
                        }
                    });

                });
            }

            services.Configure<ServiceManagementOptions>(configuration.GetSection("ServiceManagementOptions"));
            services.Configure<CentralizedLogOptions>(configuration.GetSection("CentralizedLogOptions"));
            services.AddSingleton(a => selfServerOptions);
            services.AddSingleton<IServiceContext, ServerServiceContext>();



            services.AddSingleton<IServiceManagementProvider, ServiceManagamentProvider>();
            services.AddSingleton<IMonitorServiceReportProvider, MonitorServiceReportProvider>();
            services.AddSingleton<ILogEventProvider, LogEventProvider>();
            services.AddSingleton<IMonitorProvider, MonitorProvider>();

            services.AddHostedService<CheckingLostServicesBackgroundTask>();
            services.AddHostedService<CheckingShutdownServicesBackgroundTask>();
            services.AddHostedService<GatherAllHardwareCounterServicesBackgroundTask>();

            return services;
        }

        public static IDatabaseOptionsBuilder RegisterSaturnServerRepos(this IDatabaseOptionsBuilder builder)
        {
            var databaseOptions = builder.DatabaseOptions;

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

            return builder;
        }

        public static void UseSaturnServer(
            this IApplicationBuilder app,
            IHostApplicationLifetime hostLifeTime,
            Action<SaturnServerMiddlewareOptions> action = null)
        {
            var middleOptions = new SaturnServerMiddlewareOptions();

            if (action != null)
            {
                action.Invoke(middleOptions);
            }

            if (middleOptions.UseBuiltInCors)
            {
                app.UseCors(SERVER_CORS);
            }

            if (middleOptions.UseGenerateTraceId)
            {
                app.UseMiddleware<GenerateTraceIdMiddleware>();
            }

            var hasSkipUrls = middleOptions.SkipCheckUrls != null && middleOptions.SkipCheckUrls.Length > 0;

            app.UseWhen(context =>
            {
                // TODO: We will implement more security to make sure Saturn can act well on Service Management
                var isHTTP2 = context.Request.Protocol.Equals("HTTP/2", StringComparison.OrdinalIgnoreCase);
                if (isHTTP2)
                {
                    context.Items[Core.Constants.SkipCheckPortalHeaders] = true;
                }

                if (!hasSkipUrls)
                {
                    return false;
                }
                var isSkipped = false;
                foreach (var url in middleOptions.SkipCheckUrls)
                {
                    isSkipped = context.Request.Path.ToString().Contains(url, StringComparison.Ordinal);
                    if (isSkipped)
                    {
                        break;
                    }
                }

                return !isSkipped;

            }, builder =>
            {
                if (middleOptions.AllowCheckUserSessionId)
                {
                    builder.UseMiddleware<CheckUserSessionIdMiddleware>();

                    if (middleOptions.AllowWrapException)
                    {
                        builder.UseMiddleware<NotifyExceptionLogMiddleware>();
                    }
                }

                if (middleOptions.AllowCheckTraceId)
                {
                    builder.UseMiddleware<CheckTraceIdMiddleware>();
                }
            });

            app.UseMiddleware<AddRequestMonitorMiddleware>();
            app.UseMiddleware<CatchGlobalExceptionMiddleware>();
            hostLifeTime.RegisterServiceLifecycle(app);
        }
        public static IEndpointRouteBuilder MapSaturnServer(this IEndpointRouteBuilder endpoints)
        {

            endpoints.MapGrpcService<ServiceConfigurationService>();
            endpoints.MapGrpcService<LogCollectorService>();
            endpoints.MapGrpcService<ServiceMonitorService>();
            return endpoints;
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
