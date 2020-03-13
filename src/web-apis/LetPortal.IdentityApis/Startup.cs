using LetPortal.Core;
using LetPortal.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace LetPortal.IdentityApis
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
            // Only for Development
            services.AddCors(options =>
            {
                options.AddDevCors();
                options.AddLocalCors();
                options.AddDockerLocalCors();

                options.AddPolicy("ProdCors", builder =>
                {
                    builder.WithExposedHeaders(LetPortal.Core.Constants.TokenExpiredHeader);
                });
            });

            services.AddHttpContextAccessor();
            services.AddOpenApiDocument();

            services.AddLetPortal(Configuration, options =>
            {
                options.EnableMicroservices = true;
                options.EnableSerilog = true;
                options.EnableServiceMonitor = true;
            }).AddIdentity();

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
                app.UseDevCors();
            }
            else if (env.IsLocalEnv())
            {
                app.UseLocalCors();
            }
            else if (env.IsDockerLocalEnv())
            {
                app.UseDockerLocalCors();
            }
            else
            {
                app.UseHsts();
                app.UseCors("ProdCors");
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseLetPortal(appLifetime, options =>
            {
                options.EnableCheckUserSession = true;
                options.EnableCheckTraceId = false;
                options.EnableWrapException = true;
                options.SkipCheckUrls = new string[] { "api/accounts/login", "api/accounts/forgot-password", "api/accounts/recovery-password" };
            });

            app.UseRouting();

            app.UseAuthentication();
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
