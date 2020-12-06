using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Abstractions;
using LetPortal.CMS.Core.Exceptions;
using LetPortal.CMS.Core.Providers;
using Microsoft.AspNetCore.Http;

namespace LetPortal.CMS.Core.Middlewares
{
    /// <summary>
    /// By implementing IMiddleware, we ensure this middleware will work per Request (Lifetime scope)
    /// Also the constructor can be injected Scoped Instance
    /// </summary>
    public class CheckingSiteRequestMiddleware : IMiddleware
    {
        private readonly ISiteRequestAccessor _siteRequestAccessor;

        private readonly ISiteProvider _siteProvider;

        private readonly IThemeProvider _themeProvider;

        public CheckingSiteRequestMiddleware(
            ISiteRequestAccessor siteRequest,
            ISiteProvider siteProvider,
            IThemeProvider themeProvider)
        {
            _siteRequestAccessor = siteRequest;
            _siteProvider = siteProvider;
            _themeProvider = themeProvider;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var requestDomain = context.Request.Host.Value;
            var site = await _siteProvider.LoadAsync(requestDomain);
            if(site != null)
            {
                _siteRequestAccessor.Current.Site = site;
                var theme = await _themeProvider.LoadTheme(site.ThemeId);
                _siteRequestAccessor.Current.Theme = theme;
                await next.Invoke(context);
            }  
            else
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync(ErrorCodes.NotFoundSite.MessageContent);
            }                   
        }
    }
}
