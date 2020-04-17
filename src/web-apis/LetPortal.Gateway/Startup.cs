using LetPortal.Core;
using LetPortal.Core.Logger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace LetPortal.Gateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOcelot();
            services.AddLetPortal(Configuration, options =>
            {
                options.EnableDatabaseConnection = false;
                options.EnableMicroservices = true;
                options.EnableSerilog = true;
                options.EnableServiceMonitor = true;
            })
            .AddPortalCors()
            .AddGateway();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UsePortalCors();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseMiddleware<GenerateTraceIdMiddleware>();
            app.UseLetPortal(options =>
            {
                options.EnableCheckUserSession = true;
                options.EnableCheckTraceId = true;
                options.EnableWrapException = true;
                options.SkipCheckUrls = new string[] { "api/configurations" };
            });
            app.UseLetPortalMonitor(appLifetime);
            app.UseOcelot().Wait();
        }
    }
}
