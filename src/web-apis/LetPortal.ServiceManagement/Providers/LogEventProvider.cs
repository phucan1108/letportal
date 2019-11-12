using LetPortal.Core.Logger.Models;
using LetPortal.Core.Utils;
using LetPortal.ServiceManagement.Entities;
using LetPortal.ServiceManagement.Repositories;
using System.Threading.Tasks;

namespace LetPortal.ServiceManagement.Providers
{
    public class LogEventProvider : ILogEventProvider
    {
        private readonly ILogEventRepository _logEventRepository;

        public LogEventProvider(ILogEventRepository logEventRepository)
        {
            _logEventRepository = logEventRepository;
        }

        public async Task AddLogEvent(PushLogModel pushLogModel)
        {
            var logEvent = new LogEvent
            {
                Id = DataUtil.GenerateUniqueId(),
                BeginRequest = pushLogModel.BeginRequest,
                EndRequest = pushLogModel.EndRequest,
                CpuUsage = pushLogModel.CpuUsage,
                ElapsedTime = pushLogModel.ElapsedTime,
                HttpRequestUrl = pushLogModel.HttpRequestUrl,
                HttpRequestHeaders = pushLogModel.HttpHeaders,
                HttRequestBody = pushLogModel.HttpRequestBody,
                HttpResponseStatusCode = pushLogModel.ResponseStatusCode,
                HttpResponseBody = pushLogModel.ResponseBody,
                MemoryUsed = pushLogModel.MemoryUsed,
                TraceId = pushLogModel.TraceId,
                Source = pushLogModel.ServiceName,
                SourceId = pushLogModel.RegisteredServiceId,
                StackTrace = pushLogModel.StackTraces
            };

            await _logEventRepository.AddAsync(logEvent);
        }
    }
}
