using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using LetPortal.Core.Persistences;
using LetPortal.Core.Tools;
using LetPortal.Core.Utils;
using LetPortal.Core.Versions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PersonalBlog.Tools
{
    public class PersonalBlogToolPlugin : IToolPlugin
    {
        public string AppName => "blog";

        public string Description => "Personal Blog is a simple blog";

        public IEnumerable<string> AvailableCommands => new List<string> { "*" };

        public IEnumerable<IVersion> GetAllVersions(IServiceProvider serviceProvider)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();

            return ReflectionUtil.GetAllInstances<IPersonalBlogVersion>(currentAssembly, serviceProvider);
        }

        public DatabaseOptions LoadDefaultDatabase()
        {
            return new DatabaseOptions
            {
                ConnectionType = ConnectionType.MongoDB,
                ConnectionString = "mongodb://localhost:27017",
                Datasource = "cms"
            };
        }

        public void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            // Do nothing
        }
    }
}
