using LetPortal.Core;
using LetPortal.Core.Persistences;
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
                options.AddPolicy("DevCors", builder =>
                {
                    builder.AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowAnyOrigin()
                           .AllowCredentials()
                           .WithExposedHeaders("X-Token-Expired");
                });

                options.AddPolicy("DockerLocalCors", builder =>
                {
                    builder.AllowAnyHeader()
                           .AllowAnyMethod()
                           .AllowAnyOrigin()
                           .AllowCredentials()
                           .WithExposedHeaders("X-Token-Expired");
                });

                options.AddPolicy("ProdCors", builder =>
                {
                    builder.WithExposedHeaders("X-Token-Expired");
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
            if(env.IsDevelopment())
            {
                app.UseCors("DevCors");
            }
            else if(env.IsEnvironment("DockerLocal"))
            {
                app.UseCors("DockerLocalCors");
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
