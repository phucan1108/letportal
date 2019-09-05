using Microsoft.Extensions.Configuration;

namespace LetPortal.Core.Configurations
{
    public class IntegratorConfigurationServiceSource : IConfigurationSource
    {
        private readonly ConfigurationServiceOptions _configurationServiceOptions;

        private readonly string _serviceName;

        private readonly string _version;

        public IntegratorConfigurationServiceSource(ConfigurationServiceOptions configurationServiceOptions, string serviceName, string version)
        {
            _configurationServiceOptions = configurationServiceOptions;
            _serviceName = serviceName;
            _version = version;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new IntegratorConfigurationServiceProvider(new ThroughHttpConfigurationServiceProvider(_configurationServiceOptions), _serviceName, _version);
        }
    }
}
