using System;
using System.Collections.Generic;
using System.Text;
using LetPortal.CMS.Features.Blogs.Entities;

namespace Themes.PersonalBlog.Areas.PersonalBlog.Models.Blogs
{
    public class BlogPostModel
    {
        public Blog Blog { get; set; }
        public Post Post { get; set; }
        public IEnumerable<BlogTag> Tags { get; set; }
    }
}
