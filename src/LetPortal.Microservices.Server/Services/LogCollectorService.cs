using System.Threading.Tasks;
using Grpc.Core;
using LetPortal.Core.Logger;
using LetPortal.Microservices.Server.Providers;

namespace LetPortal.Microservices.Server.Services
{
    public class LogCollectorService : LogCollector.LogCollectorService.LogCollectorServiceBase
    {
        private readonly IServiceLogger<LogCollectorService> _logger;

        private readonly ILogEventProvider _logEventProvider;

        public LogCollectorService(
            IServiceLogger<LogCollectorService> logger,
            ILogEventProvider logEventProvider)
        {
            _logger = logger;
            _logEventProvider = logEventProvider;
        }

        public override async Task<LogCollector.LogCollectorResponse> Push(IAsyncStreamReader<LogCollector.LogCollectorRequest> requestStream, ServerCallContext context)
        {
            while(await requestStream.MoveNext().ConfigureAwait(false))
            {
                await _logEventProvider.AddLogEvent(requestStream.Current);
            }

            return new LogCollector.LogCollectorResponse { Succeed = true };
        }
    }
}
