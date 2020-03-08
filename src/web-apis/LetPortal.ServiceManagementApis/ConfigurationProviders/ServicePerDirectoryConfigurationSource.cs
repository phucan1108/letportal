using Microsoft.Extensions.Configuration;

namespace LetPortal.ServiceManagementApis.ConfigurationProviders
{
    public class ServicePerDirectoryConfigurationSource : IConfigurationSource
    {
        private readonly string _directoryPath;

        private readonly string _environment;

        public ServicePerDirectoryConfigurationSource(string directoryPath, string environment)
        {
            _directoryPath = directoryPath;
            _environment = environment;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new ServicePerDirectoryConfigurationProvider(_directoryPath, _environment);
        }
    }
}
