using System.IO;
using LetPortal.Microservices.Server;
using LetPortal.Microservices.Server.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace LetPortal.Saturn
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
                    webBuilder.ConfigureKestrel(options =>
                    {
                        // Important note: If Saturn is consuming Log Stream via HTTP2, the line size should be adjusted
                        options.Limits.MaxRequestLineSize = 256 * 1024; // 256Kb
                    }).UseStartup<Startup>();
                });
        }
    }
}
