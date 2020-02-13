using LetPortal.Core;
using LetPortal.Portal;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace LetPortal.WebApis
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
            services.AddOpenApiDocument();

            services
                .AddLetPortal(Configuration, options =>
                {
                    options.EnableDatabaseConnection = true;
                    options.EnableMicroservices = true;
                    options.EnableServiceMonitor = true;
                    options.EnableSerilog = true;
                })
                .AddPortalService(options =>
                {
                    options.EnableFileServer = true;
                });

            services
                .AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    // Important note: we still use Newtonsoft instead of .NET JSON because they still don't support Timezone
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                });
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
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseLetPortal(appLifetime, options =>
            {
                options.EnableCheckUserSession = true;
                options.EnableCheckTraceId = true;
                options.EnableWrapException = true;
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
    }
}
