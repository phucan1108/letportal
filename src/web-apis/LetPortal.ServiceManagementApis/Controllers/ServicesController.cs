using LetPortal.Core.Https;
using LetPortal.Core.Services.Models;
using LetPortal.ServiceManagement.Providers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LetPortal.ServiceManagementApis.Controllers
{
    [Route("api/services")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceManagementProvider _serviceManagementProvider;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public ServicesController(
            IServiceManagementProvider serviceManagementProvider,
            IHttpContextAccessor httpContextAccessor)
        {
            _serviceManagementProvider = serviceManagementProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterService([FromBody] RegisterServiceModel registerServiceModel)
        {
            registerServiceModel.IpAddress = _httpContextAccessor.GetClientIpAddress();
            var result = await _serviceManagementProvider.RegisterService(registerServiceModel);
            return Ok(result);
        }

        [HttpPut("{serviceId}")]
        public async Task<IActionResult> UpdateRunService(string serviceId)
        {
            await _serviceManagementProvider.UpdateRunningState(serviceId);
            return Ok();
        }

        [HttpGet("shutdown/{serviceId}")]
        public async Task<IActionResult> ShutdownService(string serviceId)
        {
            await _serviceManagementProvider.ShutdownService(serviceId);
            return Ok();
        }
    }
}
