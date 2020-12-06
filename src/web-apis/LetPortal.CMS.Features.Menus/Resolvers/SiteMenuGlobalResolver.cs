using System.Threading.Tasks;
using LetPortal.CMS.Core.Abstractions;
using LetPortal.CMS.Features.Menus.Repositories;
using Microsoft.AspNetCore.Http;

namespace LetPortal.CMS.Features.Menus.Resolvers
{
    public class SiteMenuGlobalResolver : ISiteRouteGlobalResolver
    {
        private readonly ISiteMenuRepository _siteMenuRepository;

        public SiteMenuGlobalResolver(ISiteMenuRepository siteMenuRepository)
        {
            _siteMenuRepository = siteMenuRepository;
        }

        public int Order => 0;

        public Task<bool> Resolved(HttpContext httpContext, ISiteRequestAccessor request)
        {
            return Task.FromResult(true);
        }

        public async Task<bool> Resolving(HttpContext httpContext, ISiteRequestAccessor request)
        {
            var currentSite = request.Current.Site;
            var menu = await _siteMenuRepository.GetBySiteId(currentSite.Id);

            if (menu != null)
            {
                request.Current.ResolvedData.Add(Constants.SITE_MENU_DATA_KEY, menu);
            }
            return true;

        }
    }
}
