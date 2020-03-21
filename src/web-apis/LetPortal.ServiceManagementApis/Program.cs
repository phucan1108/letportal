using System.IO;
using LetPortal.ServiceManagement.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace LetPortal.ServiceManagementApis
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            

            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment.EnvironmentName;
                    var configCustomBuilder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json")
                        .AddJsonFile($"appsettings.{env}.json", true)
                        .Build();
                    var scOptions = configCustomBuilder.GetSection("ServiceConfigurationOptions").Get<ServiceConfigurationOptions>();
                    config.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath);
                    var path = Path.Combine(hostingContext.HostingEnvironment.ContentRootPath, scOptions.BasedFolder);
                    config.AddServicePerDirectory(
                        path,
                        env,
                        scOptions.SharedFolder,
                        scOptions.IgnoreCombinedServices);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}
