using System.Threading.Tasks;
using LetPortal.CMS.Features.Blogs.Entities;
using LetPortal.Core.Versions;

namespace PersonalBlog.Tools.Tags
{
    public class Tag_0_0_1 : IPersonalBlogVersion
    {
        public string VersionNumber => "1.0.0";

        public Task Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<BlogTag>("5f1abacf7a4ae23814977402");
            versionContext.DeleteData<BlogTag>("5f1ef6fce9ce36726865ebc6");

            return Task.CompletedTask;
        }

        public Task Upgrade(IVersionContext versionContext)
        {
            var dotnetTag = new BlogTag
            {
                Id = "5f1abacf7a4ae23814977402",
                Tag = ".net"
            };

            var sqlTag = new BlogTag
            {
                Id = "5f1ef6fce9ce36726865ebc6",
                Tag = "angular"
            };

            versionContext.InsertData(dotnetTag);
            versionContext.InsertData(sqlTag);

            return Task.CompletedTask;
        }
    }
}
