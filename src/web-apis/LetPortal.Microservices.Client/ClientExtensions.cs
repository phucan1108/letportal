using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using LetPortal.Core;
using LetPortal.Core.Exceptions;
using LetPortal.Core.Logger;
using LetPortal.Core.Monitors;
using LetPortal.Microservices.Client.Channels;
using LetPortal.Microservices.Client.Configurations;
using LetPortal.Microservices.Client.Middlewares;
using LetPortal.Microservices.Client.Monitors;
using LetPortal.Microservices.Client.Options;
using LetPortal.Microservices.Client.Workers;
using LetPortal.Microservices.LogCollector;
using LetPortal.Microservices.Monitors;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace LetPortal.Microservices.Client
{
    public static class ClientExtensions
    {
        const string CLIENT_CORS = "CLIENT_CORS";

        /// <summary>
        /// Allow service to load configuration from Configuration Service by servicename and version
        /// Note: If you don't have any idea to use configuration service, don't call it
        /// </summary>
        public static IConfigurationBuilder AddSaturnClientConfig(this IConfigurationBuilder builder)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            var serviceOptions = configuration.GetSection("ServiceOptions").Get<ServiceOptions>();

            if (serviceOptions == null)
            {
                throw new Exception("Missing ServiceOptions in appsettings.json, please check again!");
            }

            return builder.Add(new IntegratorConfigurationServiceSource(serviceOptions));
        }

        public static void ActsAsSaturnClient(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<SaturnClientOptions> action = null)
        {
            var saturnClientOptions = new SaturnClientOptions();
            if (action != null)
            {
                action.Invoke(saturnClientOptions);
            }

            var serviceOptions = configuration.GetSection("ServiceOptions").Get<ServiceOptions>();
            var monitorOptions = configuration.GetSection("MonitorOptions").Get<MonitorOptions>();
            if (serviceOptions != null)
            {
                services.Configure<ServiceOptions>(configuration.GetSection("ServiceOptions"));
            }

            if (saturnClientOptions.EnableServiceMonitor)
            {
                services.Configure<MonitorOptions>(configuration.GetSection("MonitorOptions"));
                services.AddSingleton<IMonitorHealthCheck, MonitorHealthCheck>();
                if (monitorOptions.Enable)
                {
                    if (monitorOptions.NotifyOptions.Enable)
                    {
                        services.Configure<HealthCheckPublisherOptions>(opts =>
                        {
                            opts.Delay = TimeSpan.FromSeconds(monitorOptions.NotifyOptions.Delay);
                        });

                        services.AddSingleton<IHealthCheckPublisher, ClientMonitorHealthCheckPublisher>();
                    }
                    services.AddHealthChecks().AddCheck<ClientMonitorHealthCheck>(Constants.CLIENT_MONITOR_HEALTHCHECK);
                }
            }

            if (saturnClientOptions.EnableSerilog)
            {
                services.Configure<LoggerOptions>(configuration.GetSection("LoggerOptions"));
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .Enrich.WithProperty("ServiceName", serviceOptions.Name)
                    .CreateLogger();

                services.AddSingleton(Log.Logger);
                services.AddTransient(typeof(IServiceLogger<>), typeof(ServiceLogger<>));
            }

            if (saturnClientOptions.EnableBuiltInCors)
            {
                var corsOptions = configuration.GetSection("CorsPortalOptions").Get<Core.Configurations.CorsPortalOptions>();
                services.AddCors(
                option =>
                {
                    option.AddPolicy(CLIENT_CORS, corsBuilder =>
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

            // Declare Channels
            services.AddSingleton<ILogCollectorChannel, LogCollectorChannel>();
            services.AddSingleton<IMonitoHeartBeatChannel, MonitorHeartBeatChannel>();

            // Declare gRPC client
            services.AddGrpcClient<MonitorService.MonitorServiceClient>(a =>
            {
                a.Address = new Uri(serviceOptions.SaturnEndpoint);
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
                if (serviceOptions.BypassSSL)
                {
                    handler.ServerCertificateCustomValidationCallback =
                            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                }
                return handler;
            });

            services.AddGrpcClient<LogCollectorService.LogCollectorServiceClient>(a =>
            {
                a.Address = new Uri(serviceOptions.SaturnEndpoint);
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
                if (serviceOptions.BypassSSL)
                {
                    handler.ServerCertificateCustomValidationCallback =
                            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                }
                return handler;
            });

            services.AddSingleton<IServiceContext, ClientServiceContext>();
            services.AddHostedService<LogCollectorBackgroundService>();
            services.AddHostedService<MonitorHealthCheckBackgrounService>();
        }

        public static void UseSaturnClient(
            this IApplicationBuilder app, 
            IHostApplicationLifetime hostLifeTime,
            Action<SaturnClientMiddlewareOptions> action = null)
        {
            var middleOptions = new SaturnClientMiddlewareOptions();

            if(action != null)
            {
                action.Invoke(middleOptions);
            }

            if (middleOptions.UseBuiltInCors)
            {
                app.UseCors(CLIENT_CORS);
            }

            if (middleOptions.UseGenerateTraceId)
            {
                app.UseMiddleware<GenerateTraceIdMiddleware>();
            }

            var hasSkipUrls = middleOptions.SkipCheckUrls != null && middleOptions.SkipCheckUrls.Length > 0;

            app.UseWhen(context =>
            {
                if(!hasSkipUrls)
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

        private static void RegisterServiceLifecycle(this IHostApplicationLifetime applicationLifetime,
            IApplicationBuilder applicationBuilder,
            Action postStartAction = null,
            Action postStopAction = null)
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
