using LetPortal.Core.Persistences;
using LetPortal.ServiceManagement;
using LetPortal.ServiceManagement.Providers;
using LetPortal.ServiceManagement.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace LetPortal.ServiceManagementApis
{
    public class Startup
    {
        private IServiceManagementProvider serviceManagementProvider;

        private bool isExistedDB = false;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddServiceManagement(Configuration);
            services
                .AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            if(serviceManagementProvider == null)
            {
                serviceManagementProvider = app.ApplicationServices.GetService<IServiceManagementProvider>();
            }

            if(!isExistedDB)
            {
                var databaseOptions = app.ApplicationServices.GetService<DatabaseOptions>();
                if(databaseOptions.ConnectionType != ConnectionType.MongoDB)
                {
                    using(var letportalDbContext = app.ApplicationServices.GetService<LetPortalServiceManagementDbContext>())
                    {
                        letportalDbContext.Database.EnsureCreated();
                        isExistedDB = true;
                    }
                }
                else
                {
                    isExistedDB = true;
                }
            }

            applicationLifetime.ApplicationStarted.Register(OnStart);
            applicationLifetime.ApplicationStopping.Register(OnStop);
            if(env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private void OnStart()
        {

        }

        private void OnStop()
        {
            serviceManagementProvider.ShutdownAllServices();
        }
    }
}
