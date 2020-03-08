using System.Threading.Tasks;
using LetPortal.Core.Logger.Models;
using LetPortal.ServiceManagement.Providers;
using Microsoft.AspNetCore.Mvc;

namespace LetPortal.ServiceManagementApis.Controllers
{
    [Route("api/logs")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly ILogEventProvider _logEventProvider;

        public LogsController(ILogEventProvider logEventProvider)
        {
            _logEventProvider = logEventProvider;
        }

        [HttpPost("")]
        public async Task<IActionResult> ConsumeLog([FromBody] PushLogModel pushLogModel)
        {
            await _logEventProvider.AddLogEvent(pushLogModel);
            return Ok();
        }

        [HttpGet("gather/{traceId}")]
        public async Task<IActionResult> GatherLog(string traceId)
        {
            await _logEventProvider.GatherAllLogs(traceId);
            return Ok();
        }
    }
}
