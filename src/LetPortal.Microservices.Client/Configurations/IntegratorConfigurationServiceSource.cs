using Microsoft.Extensions.Configuration;

namespace LetPortal.Microservices.Client.Configurations
{
    public class IntegratorConfigurationServiceSource : IConfigurationSource
    {
        private readonly ServiceOptions _serviceOptions;

        public IntegratorConfigurationServiceSource(ServiceOptions serviceOptions)
        {
            _serviceOptions = serviceOptions;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new IntegratorConfigurationServiceProvider(new ThroughGrpcConfigurationServiceProvider(_serviceOptions), _serviceOptions.Name, _serviceOptions.Version);
        }
    }
}
