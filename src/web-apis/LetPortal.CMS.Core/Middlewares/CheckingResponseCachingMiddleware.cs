using System;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Abstractions;
using LetPortal.Core.Logger;
using Microsoft.AspNetCore.Http;

namespace LetPortal.CMS.Core.Middlewares
{
    public class CheckingResponseCachingMiddleware : IMiddleware
    {
        private readonly ISiteRequestAccessor _siteRequest;

        private readonly IServiceLogger<CheckingResponseCachingMiddleware> _logger;

        public CheckingResponseCachingMiddleware(
            ISiteRequestAccessor siteRequest,
            IServiceLogger<CheckingResponseCachingMiddleware> logger)
        {
            _siteRequest = siteRequest;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {               
            if (_siteRequest.Current.Route.ResponseCaching != null && _siteRequest.Current.Route.ResponseCaching.Enable)
            {                   
                context.Response.GetTypedHeaders().CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
                {
                    MaxAge = TimeSpan.FromMinutes(_siteRequest.Current.Route.ResponseCaching.Duration),
                    Public = _siteRequest.Current.Route.ResponseCaching.CacheControl.ToLower() == "public",
                    Private = _siteRequest.Current.Route.ResponseCaching.CacheControl.ToLower() == "private",
                    NoCache = _siteRequest.Current.Route.ResponseCaching.CacheControl.ToLower() == "no-cache",
                    NoStore = _siteRequest.Current.Route.ResponseCaching.CacheControl.ToLower() == "no-store",
                };

                context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] =  new string[] { "Accept-Encoding" };                
            }
            await next.Invoke(context);            
        }
    }
}
