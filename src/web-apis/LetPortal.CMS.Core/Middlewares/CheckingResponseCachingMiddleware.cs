using System;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Abstractions;
using Microsoft.AspNetCore.Http;

namespace LetPortal.CMS.Core.Middlewares
{
    public class CheckingResponseCachingMiddleware : IMiddleware
    {
        private readonly ISiteRequestAccessor _siteRequest;

        public CheckingResponseCachingMiddleware(ISiteRequestAccessor siteRequest)
        {
            _siteRequest = siteRequest;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            await next.Invoke(context);
            if (_siteRequest.Current.Route.ResponseCaching != null && _siteRequest.Current.Route.ResponseCaching.Enable)
            {
                context.Response.GetTypedHeaders().CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
                {
                    MaxAge = new TimeSpan(0, 0, 0, _siteRequest.Current.Route.ResponseCaching.Duration),
                    Public = _siteRequest.Current.Route.ResponseCaching.CacheControl.ToLower() == "public",
                    Private = _siteRequest.Current.Route.ResponseCaching.CacheControl.ToLower() == "private",
                    NoCache = _siteRequest.Current.Route.ResponseCaching.CacheControl.ToLower() == "no-cache",
                    NoStore = _siteRequest.Current.Route.ResponseCaching.CacheControl.ToLower() == "no-store",                    
                };

                context.Response.Headers.Add("Vary", "*");
            }
        }
    }
}
