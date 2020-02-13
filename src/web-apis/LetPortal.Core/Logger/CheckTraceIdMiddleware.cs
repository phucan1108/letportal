using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace LetPortal.Core.Logger
{
    public class CheckTraceIdMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly IServiceLogger<CheckTraceIdMiddleware> _logger;

        public CheckTraceIdMiddleware(RequestDelegate next, IServiceLogger<CheckTraceIdMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var traceId = httpContext.Request.Headers[Constants.TraceIdHeader].ToString();
            if (!string.IsNullOrEmpty(traceId) || environment == "Development")
            {
                await _next.Invoke(httpContext);
            }
            else
            {
                _logger.Error($"A request is missing {Constants.TraceIdHeader} Header");
                httpContext.Response.StatusCode = 403;
                await httpContext.Response.WriteAsync("Missing some important headers");
            }
        }
    }
}
