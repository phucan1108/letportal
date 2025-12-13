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
            versionContext.DeleteData<Blog>("5fb398bd8dbe2c62e0a674ce");
            return Task.CompletedTask;
        }

        public Task Upgrade(IVersionContext versionContext)
        {
            var dotNetBlog = new Blog
            {
                Id = "5f0ab7065063a011e8fc2f8d",
                CreatedDate = DateTime.UtcNow,
                Creator = "admin",
                Title = ".NET Core",
                Description = "View all .NET Core news",
                UrlPath = "dotnet-core"
            };

            var angularBlog = new Blog
            {
                Id = "5fb398bd8dbe2c62e0a674ce",
                CreatedDate = DateTime.UtcNow,
                Creator = "admin",
                Title = "Angular",
                Description = "View new features on Angular",
                UrlPath = "angular"
            };

            versionContext.InsertData(dotNetBlog);
            versionContext.InsertData(angularBlog);

            return Task.CompletedTask;
        }
    }
}
