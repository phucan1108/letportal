using LetPortal.CMS.Core.Models;

namespace Themes.PersonalBlog.Areas.PersonalBlog.Models
{
    public class HomeCarouselModel
    {
        public MediaModel MediaSlide { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public LinkModel Link { get; set; }
    }
}
