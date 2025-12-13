using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LetPortal.ServiceManagement.Controllers
{
    [Route("api/configurations")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        private readonly ILogger _logger;

        private readonly IConfiguration _configuration;

        public ConfigurationController(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger<ConfigurationController>();
            _configuration = configuration;
        }

        [HttpGet("{serviceName}/{version}")]
        public IActionResult Get(string serviceName, string version)
        {
            var result = _configuration.GetSection($"{serviceName}:{version}").Value;
            if (!string.IsNullOrEmpty(result))
            {
                return Ok(result);
            }

            return NotFound();
        }
    }
}
