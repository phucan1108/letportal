using LetPortal.Core;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace LetPortal.Gateway
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
                    config.AddConfigurationService();
                })
                .UseStartup<Startup>();
        }
    }
}
