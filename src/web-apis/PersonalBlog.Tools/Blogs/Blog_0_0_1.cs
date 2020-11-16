using System;
using System.Threading.Tasks;
using LetPortal.CMS.Features.Blogs.Entities;
using LetPortal.Core.Versions;

namespace PersonalBlog.Tools.Blogs
{
    public class Blog_0_0_1 : IPersonalBlogVersion
    {
        public string VersionNumber => "1.0.0";

        public Task Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<Blog>("5f0ab7065063a011e8fc2f8d");
            return Task.CompletedTask;
        }

        public Task Upgrade(IVersionContext versionContext)
        {
            var blog = new Blog
            {
                Id = "5f0ab7065063a011e8fc2f8d",
                CreatedDate = DateTime.UtcNow,
                Creator = "admin",
                Title = "News",
                Description = "News on Simple Blog",
                UrlPath = "news"
            };

            versionContext.InsertData(blog);

            return Task.CompletedTask;
        }
    }
}
