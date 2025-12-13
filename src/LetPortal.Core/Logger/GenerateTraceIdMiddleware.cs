using System;
using System.Threading.Tasks;
using LetPortal.Core.Utils;
using Microsoft.AspNetCore.Http;

namespace LetPortal.Core.Logger
{
    public class GenerateTraceIdMiddleware
    {
        private readonly RequestDelegate _next;

        public GenerateTraceIdMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var traceId = httpContext.Request.Headers[Constants.TraceIdHeader].ToString();
            if (string.IsNullOrEmpty(traceId))
            {
                httpContext.Request.Headers.Add(Constants.TraceIdHeader, StringUtil.EncodeBase64FromUTF8(StringUtil.GenerateUniqueNumber()));
            }

            await _next.Invoke(httpContext);
        }
    }
}
