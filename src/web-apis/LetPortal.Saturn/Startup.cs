using System.Collections.Generic;
using LetPortal.Chat;
using LetPortal.Chat.Hubs;
using LetPortal.Core;
using LetPortal.Core.Logger;
using LetPortal.Core.Microservices.Configurations.Server;
using LetPortal.Identity;
using LetPortal.Identity.AppParts.Controllers;
using LetPortal.Microservices.Server;
using LetPortal.Portal;
using LetPortal.Portal.AppParts.Controllers;
using LetPortal.ServiceManagement;
using LetPortal.ServiceManagement.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace LetPortal.Saturn
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddMemoryCache();
            // Enable IHttpClientFactory
            services.AddHttpClient();
            services.ActAsServer(Configuration, options =>
            {
                options.EnableDatabaseOptions = true;
                options.EnableSerilog = true;
                options.EnableServiceMonitor = true;
                options.EnableBuiltInCors = true;
                options.ServerName = "Saturn";
            });

            // Changed from 0.9.0: We need to separate DatabaseOptions
            // This is very useful for reusability
            services
                .AddDatabaseOptions(Configuration)
                    .RegisterIdentityRepos()
                    .RegisterPortalRepos()
                    .RegisterSaturnServerRepos();

            // Changed from 0.9.0: AddLetPortal is just providing Builder for combining all Features
            services
                .AddLetPortal(Configuration)
                .AddIdentity()
                .AddJwtValidator(
                    hubSegments: new List<string>
                    {
                        "/chathub",
                        "/videohub"
                    })
                .AddChat()
                .AddPortalService(options =>
                {
                    options.EnableFileServer = true;
                });

            services
                .AddControllers()
                .AddApplicationPart(typeof(AppsController).Assembly)
                .AddApplicationPart(typeof(AccountsController).Assembly)
                .AddApplicationPart(typeof(ConfigurationController).Assembly)
                .AddNewtonsoftJson(options =>
                {
                    // Important note: we still use Newtonsoft instead of .NET JSON because they still don't support Timezone
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                });

            // Call SignalR when Chat is used
            services.AddSignalR();

            // Enable Grpc for Microservices
            services.AddGrpc(options =>
            {
                options.EnableDetailedErrors = true;
                options.MaxReceiveMessageSize = 1 * 1024 * 1024 * 5; // 5 MB
                options.MaxSendMessageSize = 1 * 1024 * 1024 * 5; // 5 MB
            });

            services.AddOpenApiDocument();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment() || env.IsDockerEnv())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseSaturnServer(appLifetime, options =>
            {
                options.AllowCheckTraceId = false;
                options.AllowCheckUserSessionId = false;
                options.AllowWrapException = true;
                options.UseBuiltInCors = true;
                options.UseGenerateTraceId = true;
                options.SkipCheckUrls = new string[] {
                        "api/accounts/login",
                        "api/accounts/forgot-password",
                        "api/accounts/recovery-password"};
            });

            app.UseOpenApi();
            app.UseSwaggerUi3();
            app.UseReDoc();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<HubChatClient>("/chathub", options =>
                {
                    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets | Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling;
                });
                endpoints.MapHub<HubVideoClient>("/videohub", options =>
                {
                    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets | Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling;
                });
                endpoints.MapSaturnServer();
            });
        }
    }
}
