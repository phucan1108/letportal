using System.Collections.Generic;
using LetPortal.Chat;
using LetPortal.Chat.Hubs;
using LetPortal.Chat.Repositories;
using LetPortal.Core;
using LetPortal.Core.Persistences;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LetPortal.ChatApis
{
    public class Startup
    {
        private bool isExistedDB = false;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddLetPortal(Configuration, options =>
            {
                options.EnableMicroservices = true;
                options.EnableSerilog = true;
                options.EnableServiceMonitor = true;
            })
            .AddJwtValidator( 
                    hubSegments: new List<string>
                    {
                        "/chathub",
                        "/videohub"
                    })
            .AddChat();

            services
                .AddControllers();
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime)
        {
            app.UseChatCors();

            if (!isExistedDB)
            {
                var databaseOptions = app?.ApplicationServices.GetService<DatabaseOptions>();
                if (databaseOptions.ConnectionType != ConnectionType.MongoDB)
                {
                    using var chatDbContext = app.ApplicationServices.GetService<ChatDbContext>();
                    chatDbContext.Database.EnsureCreated();
                    isExistedDB = true;
                }
                else
                {
                    isExistedDB = true;
                }
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseLetPortalMonitor(appLifetime);
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<HubChatClient>("/chathub");
                endpoints.MapHub<HubVideoClient>("/videohub");
            });
        }
    }
}
