using System.Threading.Tasks;
using LetPortal.Core.Monitors.Models;
using LetPortal.ServiceManagement.Providers;
using Microsoft.AspNetCore.Mvc;

namespace LetPortal.ServiceManagementApis.Controllers
{
    [Route("api/monitors")]
    [ApiController]
    public class MonitorsController : ControllerBase
    {
        private readonly IMonitorProvider _monitorProvider;

        public MonitorsController(IMonitorProvider monitorProvider)
        {
            _monitorProvider = monitorProvider;
        }

        [HttpPost("")]
        public async Task<IActionResult> AddHeartBeat([FromBody] PushHealthCheckModel pushHealthCheckModel)
        {
            await _monitorProvider.AddMonitorPulse(pushHealthCheckModel);
            return Ok();
        }
    }
}
