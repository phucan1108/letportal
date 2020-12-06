using System;
using System.Collections.Generic;
using System.Text;
using LetPortal.Core.Persistences;
using LetPortal.Core.Versions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LetPortal.Core.Tools
{
    /// <summary>
    /// Bootstrap interface to register with LetPortal CLI
    /// </summary>
    public interface IToolPlugin
    {
        public string AppName { get; }

        public string Description { get; }

        /// <summary>
        /// Allow to run individual commands such as: 'install', 'uninstall', 'downgrade'
        /// '*' allows to run all
        /// </summary>
        public IEnumerable<string> AvailableCommands { get; }

        /// <summary>
        /// Allow to register DI for all services, help to inject to each version
        /// </summary>
        /// <param name="services">services from CLI</param>
        /// <param name="configuration">configuration</param>
        public void RegisterServices(IServiceCollection services, IConfiguration configuration);

        /// <summary>
        /// Provide all posible versions
        /// </summary>
        /// <param name="serviceProvider">Use this service provider to construct injection</param>
        /// <returns></returns>
        public IEnumerable<IVersion> GetAllVersions(IServiceProvider serviceProvider);

        public DatabaseOptions LoadDefaultDatabase();
    }
}
