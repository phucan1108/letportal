using LetPortal.Core;
using LetPortal.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
                    builder.WithExposedHeaders(Constants.TokenExpiredHeader);
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
                .AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
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
                options.EnableWrapException = true;
                options.SkipCheckUrls = new string[] { "api/accounts/login", "api/accounts/forgot-password", "api/accounts/recovery-password" };
            });

            app.UseAuthentication();            
            app.UseMvc();
            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
    }
}
