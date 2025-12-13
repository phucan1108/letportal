using LetPortal.CMS.Core.Models;

namespace Themes.PersonalBlog.Areas.PersonalBlog.Models
{
    public class LeftFeatureModel
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public MediaModel Media { get; set; }

        public LinkModel TargetLink { get; set; }
    }
}
