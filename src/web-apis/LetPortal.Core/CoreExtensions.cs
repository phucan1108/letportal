using System;
using System.IO;
using LetPortal.Core.Configurations;
using LetPortal.Core.Exceptions;
using LetPortal.Core.Logger;
using LetPortal.Core.Monitors;
using LetPortal.Core.Persistences;
using LetPortal.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace LetPortal.Core
{
    public static class CoreExtensions
    {
        private const string DEV_CORS = "DevCors";
        private const string LOCAL_CORS = "LocalCors";
        private const string DOCK_LOCAL_CORS = "DockerLocalCors";

        public static ILetPortalBuilder AddLetPortal(this IServiceCollection serviceCollection, IConfiguration configuration, Action<LetPortalOptions> action = null)
        {
            var letPortalOptions = new LetPortalOptions();
            if (action != null)
            {
                action.Invoke(letPortalOptions);
            }

            var builder = new LetPortalBuilder(serviceCollection, configuration, letPortalOptions);

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
                builder.Services.AddHttpClient<IServiceContext, ServiceContext>();
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
                var serviceOptions = configuration.GetSection("ServiceOptions").Get<ServiceOptions>();
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .Enrich.WithProperty("ServiceName", serviceOptions.Name)
                    .CreateLogger();

                builder.Services.AddSingleton(Log.Logger);
                builder.Services.AddTransient(typeof(IServiceLogger<>), typeof(ServiceLogger<>));
            }

            return builder;
        }

        public static void UseLetPortal(
            this IApplicationBuilder app,
            IHostApplicationLifetime hostLifeTime,
            Action<LetPortalMiddlewareOptions> options = null)
        {
            var defaultOptions = new LetPortalMiddlewareOptions();
            if (options != null)
            {
                options.Invoke(defaultOptions);
            }

            hostLifeTime.RegisterServiceLifecycle(app);

            if (defaultOptions.EnableCheckUserSession || defaultOptions.EnableCheckTraceId)
            {
                if (defaultOptions.SkipCheckUrls != null && defaultOptions.SkipCheckUrls.Length > 0)
                {
                    app.UseWhen(context =>
                    {
                        var isSkipped = false;
                        foreach (var url in defaultOptions.SkipCheckUrls)
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
                        if (defaultOptions.EnableCheckUserSession)
                        {
                            builder.UseMiddleware<CheckUserSessionIdMiddleware>();
                        }

                        if (defaultOptions.EnableCheckTraceId)
                        {
                            builder.UseMiddleware<CheckTraceIdMiddleware>();
                        }
                    });
                }
                else
                {
                    if (defaultOptions.EnableCheckUserSession)
                    {
                        app.UseMiddleware<CheckUserSessionIdMiddleware>();
                    }

                    if (defaultOptions.EnableCheckTraceId)
                    {
                        app.UseMiddleware<CheckTraceIdMiddleware>();
                    }
                }

            }

            if (defaultOptions.EnableCheckUserSession
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
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var configurationServiceOptions = configuration.GetSection("ConfigurationServiceOptions").Get<ConfigurationServiceOptions>();
            var serviceOptions = configuration.GetSection("ServiceOptions").Get<ServiceOptions>();

            if (configurationServiceOptions == null || serviceOptions == null)
            {
                throw new Exception("Missing ConfiguartionServiceOptions or ServiceOptions in appsettings.json, please check again!");
            }

            return builder.Add(new IntegratorConfigurationServiceSource(configurationServiceOptions, serviceOptions.Name, serviceOptions.Version));
        }

        public static void AddDevCors(this CorsOptions options)
        {
            options?.AddPolicy(DEV_CORS, builder =>
            {
                builder.AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowAnyOrigin()
                       .WithExposedHeaders(Constants.TokenExpiredHeader);
            });
        }

        public static void AddLocalCors(this CorsOptions options)
        {
            options?.AddPolicy(LOCAL_CORS, builder =>
            {
                builder.AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowAnyOrigin()
                       .WithExposedHeaders(Constants.TokenExpiredHeader);
            });
        }

        public static void AddDockerLocalCors(this CorsOptions options)
        {
            options?.AddPolicy(DOCK_LOCAL_CORS, builder =>
            {
                builder.AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowAnyOrigin()
                       .WithExposedHeaders(Constants.TokenExpiredHeader);
            });
        }

        public static bool IsLocalEnv(this IWebHostEnvironment environment)
        {
            return environment.IsEnvironment("Local");
        }

        public static bool IsDockerLocalEnv(this IWebHostEnvironment environment)
        {
            return environment.IsEnvironment("DockerLocal");
        }

        public static IApplicationBuilder UseDevCors(this IApplicationBuilder builder)
        {
            return builder.UseCors(DEV_CORS);
        }

        public static IApplicationBuilder UseLocalCors(this IApplicationBuilder builder)
        {
            return builder.UseCors(LOCAL_CORS);
        }

        public static IApplicationBuilder UseDockerLocalCors(this IApplicationBuilder builder)
        {
            return builder.UseCors(DOCK_LOCAL_CORS);
        }

        /// <summary>
        /// Notify to Service Management when service is starting or stopping
        /// </summary>
        private static void RegisterServiceLifecycle(this IHostApplicationLifetime applicationLifetime, IApplicationBuilder applicationBuilder, Action postStartAction = null, Action postStopAction = null)
        {
            var serviceContext = applicationBuilder.ApplicationServices.GetService<IServiceContext>();
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
                });
            }
        }
    }
}
