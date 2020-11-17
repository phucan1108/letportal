using System;
using System.Collections.Generic;
using System.Text;
using LetPortal.CMS.Core.Shared;
using LetPortal.CMS.Features.Blogs.Entities;

namespace Themes.PersonalBlog.Areas.PersonalBlog.Models.Blogs
{
    public class BlogCategoryModel
    {
        public Blog Blog { get; set; }

        /// <summary>
        /// Include all paginated posts
        /// </summary>
        public PaginationData<Post> Posts { get; set; }
    }
}
