using System.Threading.Tasks;
using LetPortal.CMS.Core.Abstractions;
using LetPortal.CMS.Core.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace Themes.PersonalBlog.Areas.PersonalBlog.ViewComponents.Blogs
{
    [ThemePart(BindingType = BindingType.None)]
    public class BlogCategoriesHeaderViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync()
        {
            return Task.FromResult<IViewComponentResult>(View());
        }
    }
}
