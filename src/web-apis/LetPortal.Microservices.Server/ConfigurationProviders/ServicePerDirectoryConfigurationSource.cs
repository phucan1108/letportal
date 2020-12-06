using Microsoft.Extensions.Configuration;

namespace LetPortal.Microservices.Server.ConfigurationProviders
{
    public class ServicePerDirectoryConfigurationSource : IConfigurationSource
    {
        private readonly string _directoryPath;

        private readonly string _environment;

        private readonly string _sharedFolder;

        private readonly string _ignoreCombineSharedServices;

        public ServicePerDirectoryConfigurationSource(
            string directoryPath, 
            string environment,
            string sharedFolder,
            string ignoreCombineSharedServices)
        {
            _directoryPath = directoryPath;
            _environment = environment;
            _sharedFolder = sharedFolder;
            _ignoreCombineSharedServices = ignoreCombineSharedServices;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new ServicePerDirectoryConfigurationProvider(
                _directoryPath, 
                _environment,
                _sharedFolder,
                _ignoreCombineSharedServices);
        }
    }
}
