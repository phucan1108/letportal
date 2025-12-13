using System;
using System.Threading.Tasks;
using LetPortal.Core.Utils;
using Microsoft.AspNetCore.Http;

namespace LetPortal.Core.Logger
{
    public class CheckUserSessionIdMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly IServiceLogger<CheckUserSessionIdMiddleware> _logger;
        public CheckUserSessionIdMiddleware(RequestDelegate next, IServiceLogger<CheckUserSessionIdMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var userSessionId = httpContext.Request.Headers[Constants.UserSessionIdHeader].ToString();
            var skipCheck = httpContext.Items[Constants.SkipCheckPortalHeaders] != null ? (bool)httpContext.Items[Constants.SkipCheckPortalHeaders] : false;
            if (
                !string.IsNullOrEmpty(userSessionId)
                    || skipCheck)
            {
                httpContext.Items[Constants.UserSessionIdHeader] = StringUtil.DecodeBase64ToUTF8(userSessionId);
                await _next.Invoke(httpContext);
            }
            else
            {
                _logger.Error($"A request is missing {Constants.UserSessionIdHeader} Header");
                httpContext.Response.StatusCode = 403;
                await httpContext.Response.WriteAsync("Missing some important headers");
            }
        }
    }
}
