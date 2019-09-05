using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace LetPortal.Core.Logger
{
    public class CheckUserSessionIdMiddleware
    {
        private readonly RequestDelegate _next;

        public CheckUserSessionIdMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var userSessionId = httpContext.Request.Headers[Constants.UserSessionIdHeader].ToString();
            if(!string.IsNullOrEmpty(userSessionId) || (environment == "Development"))
            {
                await _next.Invoke(httpContext);
            }
            else
            {
                httpContext.Response.StatusCode = 403;
                await httpContext.Response.WriteAsync("Not found user session id");
            }
        }
    }
}
