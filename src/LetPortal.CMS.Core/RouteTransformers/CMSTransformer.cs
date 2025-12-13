using System.Linq;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Abstractions;
using LetPortal.CMS.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace LetPortal.CMS.Core.RouteTransformers
{
    public class CMSTransformer : DynamicRouteValueTransformer
    {
        private readonly ISiteRequestAccessor _siteRequestAccessor;

        public CMSTransformer(ISiteRequestAccessor siteRequestAccessor)
        {
            _siteRequestAccessor = siteRequestAccessor;
        }

        public async override ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
        {
            //if (values != null && values["all"] != null)
            //{
            //    var splittedUrl = values["all"].ToString().ToLower().Split("/");

            //    // Skip all static files
            //    if (new string[] { "favicon.ico", "lib", "css", "js", "fonts", "images", "videos", "medias", "assets" }.Contains(splittedUrl[0]))
            //    {
            //        return null;
            //    }
            //}

            var requestTheme = _siteRequestAccessor.Current.Theme;
            // Force routevalues back to /{theme-name}

            return await GetRouteValues(requestTheme);
        }

        private Task<RouteValueDictionary> GetRouteValues(Theme requestTheme)
        {
            return Task.FromResult(new RouteValueDictionary()
            {
                { "area", "PersonalBlog" },
                { "page", "/Index" }
            });
        }
    }
}
