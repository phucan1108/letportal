using System.Threading.Tasks;
using Grpc.Core;
using LetPortal.Core.Logger;
using LetPortal.Microservices.Configurations;
using Microsoft.Extensions.Configuration;

namespace LetPortal.Microservices.Server.Services
{
    public class ServiceConfigurationService : ServiceConfiguration.ServiceConfigurationBase
    {
        private readonly IServiceLogger<ServiceConfigurationService> _logger;

        private readonly IConfiguration _configuration;

        public ServiceConfigurationService(
            IServiceLogger<ServiceConfigurationService> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public override Task<ServiceConfigurationResponse> Pull(ServiceConfigurationRequest request, ServerCallContext context)
        {
            _logger.Info("Client is sending request", request);
            var fileContent = _configuration.GetSection($"{request.ServiceName}:{request.Version}").Value;
            return Task.FromResult(new ServiceConfigurationResponse { ConfigurationContent = fileContent });
        }
    }
}
