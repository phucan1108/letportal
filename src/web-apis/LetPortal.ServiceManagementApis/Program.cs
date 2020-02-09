using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace LetPortal.ServiceManagementApis
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Service Management is starting...");
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    Console.WriteLine("Collecting all files...");
                    config.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath);
                    var path = Path.Combine(hostingContext.HostingEnvironment.ContentRootPath, "Files");
                    Console.WriteLine("Path: " + path);
                    config.AddServicePerDirectory(path, hostingContext.HostingEnvironment.EnvironmentName);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}
