using LetPortal.Core.Persistences;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LetPortal.Core
{
    class LetPortalBuilder : ILetPortalBuilder
    {
        public IServiceCollection Services { get; }

        public IConfiguration Configuration { get; }

        public LetPortalOptions LetPortalOptions { get; }

        public ConnectionType ConnectionType { get; private set; }

        public IHealthChecksBuilder HealthChecksBuilder { get; private set; }

        public LetPortalBuilder(
            IServiceCollection serviceCollection,
            IConfiguration configuration,
            LetPortalOptions letPortalOptions)
        {
            Services = serviceCollection;
            Configuration = configuration;
            LetPortalOptions = letPortalOptions;
        }

        public void SetConnectionType(ConnectionType connectionType)
        {
            ConnectionType = connectionType;
        }

        public void SetHealthCheckBuilder(IHealthChecksBuilder healthChecksBuilder)
        {
            HealthChecksBuilder = healthChecksBuilder;
        }

        public void Build()
        {

        }
    }
}
