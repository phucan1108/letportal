using LetPortal.Core.Https;
using LetPortal.Core.Logger;
using LetPortal.Core.Logger.Models;
using LetPortal.Core.Monitors;
using LetPortal.Core.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LetPortal.Core.Services
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
            if(httpContext.Request.Headers.Any(a => a.Key == Constants.TraceIdHeader)
                && httpContext.Request.Headers.Any(a => a.Key == Constants.UserSessionIdHeader)
                && loggerOptions.CurrentValue.NotifyOptions.Enable
                && (loggerOptions.CurrentValue.NotifyOptions.StatusCodes.Contains(httpContext.Response.StatusCode)
                    || loggerOptions.CurrentValue.NotifyOptions.Urls
                        .Any(a => httpContext.Request.Path.ToString().Contains(a.UrlPath) && a.Enable && a.StatusCodes.Contains(httpContext.Response.StatusCode))))
            {
                Process currentProcess = Process.GetCurrentProcess();
                object reponseErrorBody;
                var rawBody = await httpContext.Request.GetRawBodyAsync();
                var pushLogModel = new PushLogModel
                {
                    TraceId = httpContext.Request.Headers[Constants.TraceIdHeader].ToString(),
                    UserSessionId = httpContext.Request.Headers[Constants.UserSessionIdHeader].ToString(),
                    HttpRequestUrl = httpContext.Request.Path.ToUriComponent(),
                    HttpHeaders = ConvertUtil.SerializeObject(httpContext.Request.Headers),
                    HttpRequestBody = rawBody,
                    ResponseBody = httpContext.Items.TryGetValue(Constants.OccurredException, out reponseErrorBody) ? reponseErrorBody.ToString() : await httpContext.Response.GetRawBodyAsync(),
                    ResponseStatusCode = httpContext.Response.StatusCode
                };

                if(monitorOptions.CurrentValue.Enable)
                {
                    var requestMonitor = httpContext.Items[Constants.RequestMonitor] as RequestMonitor;
                    pushLogModel.BeginRequest = requestMonitor.BeginDateTime;
                    pushLogModel.EndRequest = requestMonitor.EndDateTime;
                    pushLogModel.ElapsedTime = requestMonitor.ElapsedTime;
                    pushLogModel.CpuUsage = requestMonitor.CpuUsage;
                    pushLogModel.MemoryUsed = requestMonitor.MemoryUsed;
                }

                serviceContext.PushLog(pushLogModel);
            }
        }
    }
}
