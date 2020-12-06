using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.CMS.Core;
using LetPortal.CMS.Core.RouteTransformers;
using LetPortal.Core;
using LetPortal.Microservices.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LetPortal.Websites.CMS
{
    public class Startup
    {
        IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.ActsAsSaturnClient(Configuration, options =>
            {
                options.EnableBuiltInCors = true;
                options.EnableSerilog = true;
                options.EnableServiceMonitor = true;
            });

            services.AddDatabaseOptions(Configuration)
                    .RegisterCMSRepos();

            services.AddCMS(Configuration);

            services
                .AddRazorPages()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AddLetPortalCMS(Configuration);                    
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSaturnClient(appLifetime, options =>
            {
                options.UseBuiltInCors = true;
            });
            app.UseRouting();
            app.UseLetPortalCMS();
            //app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            { 
                endpoints.MapRazorPages();
            });
        }
    }
}
