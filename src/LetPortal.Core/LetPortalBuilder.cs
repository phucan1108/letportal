using LetPortal.Core.Configurations;
using LetPortal.Core.Persistences;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LetPortal.Core
{
    public class LetPortalBuilder : ILetPortalBuilder
    {
        public IServiceCollection Services { get; }

        public IConfiguration Configuration { get; }

        public LetPortalOptions LetPortalOptions { get; }

        public CorsPortalOptions CorsOptions { get; }

        public ConnectionType ConnectionType { get; private set; }

        public IHealthChecksBuilder HealthChecksBuilder { get; private set; }

        public LetPortalBuilder(
            IServiceCollection serviceCollection,
            IConfiguration configuration,
            LetPortalOptions letPortalOptions,
            CorsPortalOptions corsOptions)
        {
            Services = serviceCollection;
            Configuration = configuration;
            LetPortalOptions = letPortalOptions;
            CorsOptions = corsOptions;
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
