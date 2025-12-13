using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core;
using LetPortal.Core.Https;
using LetPortal.Core.Logger;
using LetPortal.Core.Monitors;
using LetPortal.Core.Utils;
using LetPortal.Microservices.LogCollector;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace LetPortal.Microservices.Server.Middlewares
{
    public class NotifyExceptionLogMiddleware
    {
        private readonly RequestDelegate _next;

        public NotifyExceptionLogMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext httpContext,
            IOptionsMonitor<MonitorOptions> monitorOptions,
            IOptionsMonitor<LoggerOptions> loggerOptions,
            IServiceContext serviceContext)
        {
            await _next.Invoke(httpContext);

            // Ensure loggerOptions is enable and allow to push log
            if (httpContext.Request.Headers.Any(a => a.Key == Core.Constants.TraceIdHeader)
                && httpContext.Request.Headers.Any(a => a.Key == Core.Constants.UserSessionIdHeader)
                && loggerOptions.CurrentValue.NotifyOptions.Enable
                && (loggerOptions.CurrentValue.NotifyOptions.StatusCodes.Contains(httpContext.Response.StatusCode)
                    || loggerOptions.CurrentValue.NotifyOptions.Urls
                        .Any(a => httpContext.Request.Path.ToString().Contains(a.UrlPath) && a.Enable && a.StatusCodes.Contains(httpContext.Response.StatusCode))))
            {
                var currentProcess = Process.GetCurrentProcess();
                var rawBody = await httpContext.Request.GetRawBodyAsync();
                var logCollectorRequest = new LogCollectorRequest
                {
                    TraceId = StringUtil.DecodeBase64ToUTF8(httpContext.Request.Headers[Core.Constants.TraceIdHeader].ToString()),
                    UserSessionId = StringUtil.DecodeBase64ToUTF8(httpContext.Request.Headers[Core.Constants.UserSessionIdHeader].ToString()),
                    HttpRequestUrl = httpContext.Request.Path.ToUriComponent(),
                    HttpHeaders = ConvertUtil.SerializeObject(httpContext.Request.Headers),
                    HttpRequestBody = rawBody,
                    ResponseBody = httpContext.Items.TryGetValue(Core.Constants.OccurredException, out var reponseErrorBody) ? reponseErrorBody.ToString() : await httpContext.Response.GetRawBodyAsync(),
                    ResponseStatusCode = httpContext.Response.StatusCode
                };

                if (monitorOptions.CurrentValue.Enable)
                {
                    var requestMonitor = httpContext.Items[Core.Constants.RequestMonitor] as RequestMonitor;
                    logCollectorRequest.BeginRequest = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(requestMonitor.BeginDateTime);
                    logCollectorRequest.EndRequest = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(requestMonitor.EndDateTime);
                    logCollectorRequest.ElapsedTime = requestMonitor.ElapsedTime;
                    logCollectorRequest.CpuUsage = requestMonitor.CpuUsage;
                    logCollectorRequest.MemoryUsed = requestMonitor.MemoryUsed;
                }

                _ = Task.Run(() => serviceContext.PushLog(logCollectorRequest));
            }
        }
    }
}
