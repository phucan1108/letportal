using System;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Abstractions;
using LetPortal.CMS.Core.Exceptions;
using LetPortal.CMS.Core.Providers;
using LetPortal.CMS.Core.Shared;
using Microsoft.AspNetCore.Http;

namespace LetPortal.CMS.Core.Middlewares
{
    public class CheckingPageRequestMiddleware : IMiddleware
    {
        private readonly ISiteRequestAccessor _siteRequestAccessor;

        private readonly ISiteRouteProvider _siteRouteProvider;

        private readonly IThemeProvider _themeProvider;

        private readonly IPageProvider _pageProvider;

        public CheckingPageRequestMiddleware(
            ISiteRequestAccessor siteRequestAccessor,
            ISiteRouteProvider siteRouteProvider,
            IThemeProvider themeProvider,
            IPageProvider pageProvider)
        {
            _siteRequestAccessor = siteRequestAccessor;
            _siteRouteProvider = siteRouteProvider;
            _pageProvider = pageProvider;
            _themeProvider = themeProvider;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            //await next.Invoke(context);
            var requestPath = context.Request.Path.Value;
            var routeMap = await _siteRouteProvider
                .FindRouteAsync(_siteRequestAccessor.Current.Site.Id, "default", requestPath);

            if (routeMap != null)
            {
                _siteRequestAccessor.Current.Route = routeMap;
                _siteRequestAccessor.Current.RouteValues = routeMap.LoadRouteValues(requestPath);

                var page = await _pageProvider.LoadPage(routeMap.PageId);
                _siteRequestAccessor.Current.Page = page;

                var pageTemplate = await _pageProvider.LoadPageTemplate(page.PageTemplateId);
                _siteRequestAccessor.Current.PageTemplate = pageTemplate;

                if(context.Request.Query.Count > 0)
                {
                    foreach(var query in context.Request.Query)
                    {
                        _siteRequestAccessor.Current.QueryParams.Add(query.Key, query.Value);
                    }
                }

                // Combine for pagination
                _siteRequestAccessor.Current.Pagination = new Pagination
                {
                    CurrentPage = context.Request.Query.ContainsKey("page") ? Convert.ToInt32(context.Request.Query["page"].ToString()) : 1,
                    NumberPerPage = context.Request.Query.ContainsKey("size") ? Convert.ToInt32(context.Request.Query["size"].ToString()) : 10,
                    MaximumPage = 5
                };

                await next.Invoke(context);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync(ErrorCodes.NotFoundSiteRoute.MessageContent);
            }
        }
    }
}
