using System.Threading.Tasks;
using LetPortal.CMS.Core.Entities;
using LetPortal.Core.Versions;

namespace PersonalBlog.Tools.Pages
{
    public class Page_0_0_1 : IPersonalBlogVersion
    {
        public string VersionNumber => "0.0.1";

        public Task Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<Page>("5f02cb57e63b2b3634d51375");
            versionContext.DeleteData<Page>("5f0aa3731558bf76447b18d0");
            versionContext.DeleteData<Page>("5f0aa3731558bf76447b18d1");
            versionContext.DeleteData<Page>("5f0aa3731558bf76447b18d3");
            versionContext.DeleteData<Page>("5f1ef6fce9ce36726865ebc3");
            return Task.CompletedTask;
        }

        public Task Upgrade(IVersionContext versionContext)
        {
            var rootPage = new Page
            {
                Id = "5f02cb57e63b2b3634d51375",
                GoogleMetadata = new GoogleMetadata
                {
                    Title = "Personal Blog - Home",
                    Description = "Personal Blog is a simple blog",
                    AllowGoogleRead = true
                },
                Name = "Home",
                Enable = true,
                PageTemplateId = "5f0aa3731558bf76447b18cb",
                ChosenPageVersionId = "5f108cac188c2043dca459b3",
                SiteId = Constants.MAIN_SITE_ID
            };

            var blogsPage = new Page
            {
                Id = "5f0aa3731558bf76447b18d0",
                GoogleMetadata = new GoogleMetadata
                {
                    Title = "Personal Blog - Blogs",
                    Description = "Personal Blog Blogs",
                    AllowGoogleRead = true
                },
                Name = "Blogs",
                Enable = true,
                PageTemplateId = "5f0aa3731558bf76447b18cc",
                SiteId = Constants.MAIN_SITE_ID
            };

            var blogCatePage = new Page
            {
                Id = "5f0aa3731558bf76447b18d1",
                GoogleMetadata = new GoogleMetadata
                {
                    Title = "{{blog.title}}",
                    Description = "{{blog.description}}",
                    AllowGoogleRead = true
                },
                Name = "Blog Category",
                Enable = true,
                PageTemplateId = "5f0aa3731558bf76447b18cd",
                SiteId = Constants.MAIN_SITE_ID
            };

            var blogPostPage = new Page
            {
                Id = "5f0aa3731558bf76447b18d3",
                GoogleMetadata = new GoogleMetadata
                {
                    Title = "{{blogPost.title}}",
                    Description = "{{blogPost.description}}",
                    AllowGoogleRead = true
                },
                Name = "Blog Post",
                Enable = true,
                PageTemplateId = "5f0aa3731558bf76447b18ce",
                SiteId = Constants.MAIN_SITE_ID
            };

            var blogTagsPage = new Page
            {
                Id = "5f1ef6fce9ce36726865ebc3",
                GoogleMetadata = new GoogleMetadata
                {
                    Title = "Personal Blog - Tags",
                    Description = "Personal Blog - Tags",
                    AllowGoogleRead = true
                },
                Name = "Blog Tags",
                Enable = true,
                PageTemplateId = "5f1ef6fce9ce36726865ebc0",
                SiteId = Constants.MAIN_SITE_ID
            };

            versionContext.InsertData(rootPage);
            versionContext.InsertData(blogsPage);
            versionContext.InsertData(blogCatePage);
            versionContext.InsertData(blogPostPage);
            versionContext.InsertData(blogTagsPage);

            return Task.CompletedTask;
        }
    }
}
