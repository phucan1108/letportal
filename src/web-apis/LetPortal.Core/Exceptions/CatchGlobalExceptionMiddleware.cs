using System;
using System.Net;
using System.Threading.Tasks;
using LetPortal.Core.Logger;
using Microsoft.AspNetCore.Http;

namespace LetPortal.Core.Exceptions
{
    public class CatchGlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public CatchGlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext httpContext,
            IServiceLogger<CatchGlobalExceptionMiddleware> serviceLogger)
        {
            try
            {
                await _next.Invoke(httpContext);
            }
            catch (Exception ex)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                httpContext.Response.ContentType = "application/json";
                if (ex is CoreException coreException)
                {

                    serviceLogger.Warning(coreException, "Wrong business flow with error {$coreException}", coreException);
                    httpContext.Items[Constants.OccurredException] = coreException;
                    await httpContext.Response.WriteAsync(coreException.ToJsonString());
                }
                else
                {
                    serviceLogger.Critical(ex, "Internal server error {$error}", ex);
                    var responseBody = (new CoreException(ErrorCodes.InternalException)).ToJsonString();
                    httpContext.Items[Constants.OccurredException] = responseBody;
                    await httpContext.Response.WriteAsync(responseBody);
                }
            }
        }
    }
}
