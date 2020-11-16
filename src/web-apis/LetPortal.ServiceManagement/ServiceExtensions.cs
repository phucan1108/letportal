using LetPortal.ServiceManagement.ConfigurationProviders;
using Microsoft.Extensions.Configuration;

namespace LetPortal.ServiceManagement
{
    public static class ServiceExtensions
    {
        public static IConfigurationBuilder AddServicePerDirectory(this IConfigurationBuilder builder, 
            string directoryPath, 
            string environment,
            string sharedFolder,
            string ignoreCombineSharedServices)
        {
            return builder.Add(new ServicePerDirectoryConfigurationSource(directoryPath, environment, sharedFolder, ignoreCombineSharedServices));
        }
    }
}
