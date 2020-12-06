using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Entities;
using LetPortal.Core.Versions;

namespace PersonalBlog.Tools.SiteRoutes
{
    public class SiteRoute_0_0_1 : IPersonalBlogVersion
    {
        public string VersionNumber => "1.0.0";

        public Task Downgrade(IVersionContext versionContext)
        {
            versionContext.DeleteData<SiteRoute>("5f02cb57e63b2b3634d51373");
            versionContext.DeleteData<SiteRoute>("5f02cb57e63b2b3634d51374");
            versionContext.DeleteData<SiteRoute>("5f0ab7065063a011e8fc2f85");
            versionContext.DeleteData<SiteRoute>("5f0ab7065063a011e8fc2f86");
            versionContext.DeleteData<SiteRoute>("5f204bc8d5ac7c6bd0dde43e");
            return Task.CompletedTask;
        }

        public Task Upgrade(IVersionContext versionContext)
        {
            var rootTaliSiteRoute = new SiteRoute
            {
                Id = "5f02cb57e63b2b3634d51373",
                ElemPath = "/",
                PageId = "5f02cb57e63b2b3634d51375",
                LocaleId = "default",
                ParentId = "5f02cb57e63b2b3634d51373",
                RoutePath = "/",
                SiteId = Constants.MAIN_SITE_ID
            };

            var blogsListSiteRoute = new SiteRoute
            {
                Id = "5f02cb57e63b2b3634d51374",
                ElemPath = "blogs",
                PageId = "5f0aa3731558bf76447b18d0",
                LocaleId = "default",
                ParentId = "5f02cb57e63b2b3634d51373",
                RoutePath = "/blogs",
                ResolveMaps = new List<ResolveDataMap>
               {
                   new ResolveDataMap
                   {
                       Key = "blogs",
                       Inputs = "",
                       Resolver = "blogs"
                   }
               },
                SiteId = Constants.MAIN_SITE_ID
            };

            var blogCateSiteRoute = new SiteRoute
            {
                Id = "5f0ab7065063a011e8fc2f85",
                ElemPath = "*",
                RoutePath = "/blogs/*",
                WildcardKey = "blog",
                LocaleId = "default",
                PageId = "5f0aa3731558bf76447b18d1",
                ParentId = "5f02cb57e63b2b3634d51374",
                ResolveMaps = new List<ResolveDataMap>
                {
                    new ResolveDataMap
                    {
                        Key = "blog",
                        Inputs = "blog",
                        Resolver = "blog"
                    },
                    new ResolveDataMap
                    {
                       Key = "posts",
                       Resolver = "postsOfBlog"
                    }
                },
                SiteId = Constants.MAIN_SITE_ID
            };

            var postsEmptySiteRoute = new SiteRoute
            {
                Id = "5f204bc8d5ac7c6bd0dde43e",
                ElemPath = "posts",
                RoutePath = "/posts",
                LocaleId = "default",
                ParentId = "5f02cb57e63b2b3634d51373",
                SiteId = Constants.MAIN_SITE_ID
            };

            var blogPostSiteRoute = new SiteRoute
            {
                Id = "5f0ab7065063a011e8fc2f86",
                ElemPath = "*",
                RoutePath = "/posts/*",
                WildcardKey = "blogPost",
                LocaleId = "default",
                PageId = "5f0aa3731558bf76447b18d3",
                ParentId = "5f204bc8d5ac7c6bd0dde43e",
                ResponseCaching = new ResponseCaching
                {
                    Enable = true,
                    CacheControl = "public",
                    Duration = 10
                },
                ResolveMaps = new List<ResolveDataMap>
                {
                    new ResolveDataMap
                    {
                        Key = "blogPost",
                        Inputs = "blogPost",
                        Resolver = "blogPost"
                    }
                },
                SiteId = Constants.MAIN_SITE_ID
            };

            var blogTagsSiteRoute = new SiteRoute
            {
                Id = "5f1ef6fce9ce36726865ebc2",
                ElemPath = "blog-tagged",
                RoutePath = "/blog-tagged",
                LocaleId = "default",
                PageId = "5f1ef6fce9ce36726865ebc3",
                ParentId = "5f02cb57e63b2b3634d51373",
                ResolveMaps = new List<ResolveDataMap>
                {
                    new ResolveDataMap
                    {
                       Key = "posts",
                       Resolver = "postsOfTag",
                       EnableInMemoryCache = true,
                       CacheDuration = 60
                    }
                },
                SiteId = Constants.MAIN_SITE_ID
            };

            versionContext.InsertData(rootTaliSiteRoute);
            versionContext.InsertData(blogsListSiteRoute);
            versionContext.InsertData(blogCateSiteRoute);
            versionContext.InsertData(blogPostSiteRoute);
            versionContext.InsertData(blogTagsSiteRoute);
            versionContext.InsertData(postsEmptySiteRoute);

            return Task.CompletedTask;
        }
    }
}
