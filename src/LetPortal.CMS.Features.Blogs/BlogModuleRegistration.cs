using System.Collections.Generic;
using LetPortal.CMS.Core.Abstractions;
using LetPortal.CMS.Features.Blogs.Repositories.Blogs;
using LetPortal.CMS.Features.Blogs.Repositories.BlogTags;
using LetPortal.CMS.Features.Blogs.Repositories.Posts;
using LetPortal.CMS.Features.Blogs.Resolvers;
using LetPortal.Core;
using LetPortal.Core.Persistences;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LetPortal.CMS.Features.Blogs
{
    public class BlogModuleRegistration : IModuleRegistration
    {
        public string Name => "BlogModule";

        public string Description => "Provides all business for Blog Page";

        public IEnumerable<string> RequiredParts => new List<string>
        {
            "BlogList",
            "BlogDetail"
        };

        public void Register(
            IServiceCollection services, 
            IConfiguration configuration,
            DatabaseOptions databaseOptions)
        {
            if(databaseOptions.ConnectionType == LetPortal.Core.Persistences.ConnectionType.MongoDB)
            {
                services.AddSingleton<IBlogRepository, BlogMongoRepository>();
                services.AddSingleton<IBlogTagRepository, BlogTagMongoRepository>();
                services.AddSingleton<IPostRepository, PostMongoRepository>();
            }

            services.AddTransient<ISiteRouteResolver, BlogResolver>();
            services.AddTransient<ISiteRouteResolver, PostResolver>();
            services.AddTransient<ISiteRouteResolver, BlogsResolver>();
            services.AddTransient<ISiteRouteResolver, PostsOfBlogResolver>();
            services.AddTransient<ISiteRouteResolver, PostsOfTagResolver>();
        }
    }
}
