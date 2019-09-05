using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace LetPortal.Core.Logger
{
    public class CheckTraceIdMiddleware
    {
        private readonly RequestDelegate _next;

        public CheckTraceIdMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var traceId = httpContext.Request.Headers[Constants.TraceIdHeader].ToString();
            if(!string.IsNullOrEmpty(traceId) || (environment == "Development"))
            {
                await _next.Invoke(httpContext);
            }
            else
            {
                httpContext.Response.StatusCode = 403;
                await httpContext.Response.WriteAsync("Not found trace id");
            }
        }
    }
}
