using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Abstractions;
using LetPortal.CMS.Features.Menus.Entities;
using Microsoft.AspNetCore.Mvc;
using Themes.PersonalBlog.Areas.PersonalBlog.Models;

namespace Themes.PersonalBlog.Areas.PersonalBlog.ViewComponents.SharedSection
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly ISiteRequestAccessor _siteRequest;

        public MenuViewComponent(ISiteRequestAccessor siteRequest)
        {
            _siteRequest = siteRequest;
        }

        public Task<IViewComponentResult> InvokeAsync()
        {
            var menus = new List<MenuModel>();
            if (_siteRequest.Current.ResolvedData.TryGetValue(LetPortal.CMS.Features.Menus.Constants.SITE_MENU_DATA_KEY, out object outObject))
            {
                SiteMenu siteMenu = (SiteMenu)outObject;
                if (siteMenu.Menus != null && siteMenu.Menus.Count > 0)
                {
                    foreach (var menu in siteMenu.Menus)
                    {
                        menus.Add(new MenuModel { Name = menu.Name, Url = menu.Url, Sub = GetChildren(menu) });
                    }
                }
            }
           
            return Task.FromResult<IViewComponentResult>(View(menus));
        }

        private IEnumerable<MenuModel> GetChildren(Menu menu)
        {
            if(menu.Sub != null && menu.Sub.Count > 0)
            {
                var sub = new List<MenuModel>();
                foreach(var subMenu in menu.Sub)
                {
                    sub.Add(new MenuModel { Name = subMenu.Name, Url = subMenu.Url, Sub = GetChildren(subMenu) });
                }
                return sub;
            }

            return Enumerable.Empty<MenuModel>();
        }
    }
}
