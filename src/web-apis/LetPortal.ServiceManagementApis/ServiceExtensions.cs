using LetPortal.ServiceManagementApis.ConfigurationProviders;
using Microsoft.Extensions.Configuration;

namespace LetPortal.ServiceManagementApis
{
    public static class ServiceExtensions
    {
        public static IConfigurationBuilder AddServicePerDirectory(this IConfigurationBuilder builder, string directoryPath, string environment)
        {
            return builder.Add(new ServicePerDirectoryConfigurationSource(directoryPath, environment));
        }
    }
}
