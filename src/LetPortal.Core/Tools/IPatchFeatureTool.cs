using System;
using System.Collections.Generic;
using LetPortal.Core.Versions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LetPortal.Core.Tools
{
    public interface IPatchFeatureTool<T> where T : IVersion
    {
        string PatchName { get; }

        IEnumerable<T> GetAllVersions(IServiceProvider serviceProvider);

        /// <summary>
        /// Allow to register DI for all services, help to inject to each version
        /// </summary>
        /// <param name="services">services from CLI</param>
        /// <param name="configuration">configuration</param>
        public void RegisterServices(IServiceCollection services, IConfiguration configuration);
    }
}
