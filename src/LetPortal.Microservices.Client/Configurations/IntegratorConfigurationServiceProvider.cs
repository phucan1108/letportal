using Microsoft.Extensions.Configuration;

namespace LetPortal.Microservices.Client.Configurations
{
    public class IntegratorConfigurationServiceProvider : ConfigurationProvider
    {
        private readonly IConfigurationServiceProvider _configurationServiceProvider;

        private readonly string _serviceName;
        private readonly string _version;

        public IntegratorConfigurationServiceProvider(
            IConfigurationServiceProvider configurationServiceProvider,
            string serviceName,
            string version)
        {
            _configurationServiceProvider = configurationServiceProvider;
            _serviceName = serviceName;
            _version = version;
        }

        public override void Load()
        {
            var loadingDic = _configurationServiceProvider.GetConfiguration(_serviceName, _version).Result;
            foreach (var kvp in loadingDic)
            {
                Data.Add(kvp);
            }
        }
    }
}
