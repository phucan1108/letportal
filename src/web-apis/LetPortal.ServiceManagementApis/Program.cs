using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace LetPortal.ServiceManagementApis
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "Files");
                    config.AddServicePerDirectory(path, hostingContext.HostingEnvironment.EnvironmentName);
                })
                .UseStartup<Startup>();
        }
    }
}
