using System.Threading.Tasks;
using LetPortal.CMS.Core.Abstractions;
using LetPortal.CMS.Core.Providers;
using Microsoft.AspNetCore.Http;

namespace LetPortal.CMS.Core.Middlewares
{
    public class CheckingPageVersionRequestMiddleware : IMiddleware
    {
        private readonly ISiteRequestAccessor _siteRequest;

        private readonly IPageProvider _pageProvider;

        public CheckingPageVersionRequestMiddleware(
            ISiteRequestAccessor siteRequest,
            IPageProvider pageProvider)
        {
            _siteRequest = siteRequest;
            _pageProvider = pageProvider;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!string.IsNullOrEmpty(_siteRequest.Current.Page.ChosenPageVersionId))
            {
                var pageVersion = await _pageProvider.LoadPageVersion(_siteRequest.Current.Page.ChosenPageVersionId);
                _siteRequest.Current.PageVersion = pageVersion;
            }
            await next.Invoke(context);
        }
    }
}
