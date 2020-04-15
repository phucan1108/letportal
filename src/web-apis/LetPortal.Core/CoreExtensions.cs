using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LetPortal.Core.Configurations;
using LetPortal.Core.Exceptions;
using LetPortal.Core.Logger;
using LetPortal.Core.Monitors;
using LetPortal.Core.Persistences;
using LetPortal.Core.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace LetPortal.Core
{
    public static class CoreExtensions
    {
        private const string PORTAL_CORS = "LETPortalCors";

        public static ILetPortalBuilder AddLetPortal(this IServiceCollection serviceCollection, IConfiguration configuration, Action<LetPortalOptions> action = null)
        {
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
                corsOptions);

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

        public static ILetPortalBuilder AddJwtValidator(this ILetPortalBuilder builder, Configurations.JwtBearerOptions jwtOptions = null, List<string> hubSegments = null)
        {
            if(jwtOptions == null)
            {
                builder.Services.Configure<Core.Configurations.JwtBearerOptions>(builder.Configuration.GetSection("JwtBearerOptions"));
                jwtOptions = builder.Configuration.GetSection("JwtBearerOptions").Get<Core.Configurations.JwtBearerOptions>();
            }            
            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtOptions.Secret)),
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    RequireSignedTokens = true,
                    // Be careful, if we use 'sub' as username, so we need to set http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier
                    // Because .NET Core Identity will map sub -> nameidentifier
                    NameClaimType = ClaimTypes.NameIdentifier,
                    // Important for testing purpose with zero but in production, it should be 5m (default)
                    ClockSkew =                        
                        Environment
                            .GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development" ?
                            TimeSpan.Zero : TimeSpan.FromMinutes(5)

                };
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) && hubSegments != null)
                        {
                            if(hubSegments.Any(url => (path.StartsWithSegments(url, StringComparison.OrdinalIgnoreCase))))
                            {   
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }
                        }
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("X-Token-Expired", "true");
                        }
                        else
                        {
                            Console.WriteLine("There are some unexpected erros while trying to validate JWT token. Exception: " + context.Exception.ToString());
                        }
                        return Task.CompletedTask;
                    }
                };
            });

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

        public static ILetPortalBuilder AddPortalCors(this ILetPortalBuilder builder)
        {
            builder.Services.AddCors(
                option =>
                {
                    option.AddPolicy(PORTAL_CORS, corsBuilder =>
                    {
                        if (builder.CorsOptions.AllowAny)
                        {
                            corsBuilder.AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowAnyOrigin()
                               .WithExposedHeaders(Constants.TokenExpiredHeader);
                        }
                        else
                        {
                            if (builder.CorsOptions.AllowAnyHeader)
                            {
                                corsBuilder.AllowAnyHeader();
                            }
                            else
                            {
                                corsBuilder.WithHeaders(builder.CorsOptions.AllowedHeaders.ToArray());
                            }

                            if (builder.CorsOptions.AllowAnyMethod)
                            {
                                corsBuilder.AllowAnyMethod();
                            }
                            else
                            {
                                corsBuilder.WithMethods(builder.CorsOptions.AllowedMethods.ToArray());
                            }

                            if (builder.CorsOptions.AllowAnyHost)
                            {
                                corsBuilder.AllowAnyOrigin();
                            }
                            else
                            {
                                corsBuilder.WithOrigins(builder.CorsOptions.AllowedHosts.ToArray());
                            }

                            if (builder.CorsOptions.ExposedHeaders != null)
                            {
                                corsBuilder.WithExposedHeaders(builder.CorsOptions.ExposedHeaders.ToArray());
                            }
                        }
                    });

                });
            return builder;
        }

        public static void UsePortalCors(this IApplicationBuilder app)
        {
            app.UseCors(PORTAL_CORS);
        }  

        public static bool IsLocalEnv(this IWebHostEnvironment environment)
        {
            return environment.IsEnvironment("Local");
        }

        public static bool IsDockerEnv(this IWebHostEnvironment environment)
        {
            return environment.IsEnvironment("Docker");
        }

        public static bool IsDockerLocalEnv(this IWebHostEnvironment environment)
        {
            return environment.IsEnvironment("DockerLocal");
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
