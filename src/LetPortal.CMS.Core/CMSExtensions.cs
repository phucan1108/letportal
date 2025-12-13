using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using LetPortal.CMS.Core.Abstractions;
using LetPortal.CMS.Core.Configurations;
using LetPortal.CMS.Core.Implements;
using LetPortal.CMS.Core.Middlewares;
using LetPortal.CMS.Core.Providers;
using LetPortal.CMS.Core.Repositories.Pages;
using LetPortal.CMS.Core.Repositories.SiteRoutes;
using LetPortal.CMS.Core.Repositories.Sites;
using LetPortal.CMS.Core.Repositories.Themes;
using LetPortal.CMS.Core.Routers;
using LetPortal.CMS.Core.RouteTransformers;
using LetPortal.CMS.Core.Services.Pages;
using LetPortal.CMS.Core.TagHelpers;
using LetPortal.CMS.Core.Utils;
using LetPortal.Core;
using LetPortal.Core.Persistences;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LetPortal.CMS.Core
{
    public static class CMSExtensions
    {
        public static IDatabaseOptionsBuilder RegisterCMSRepos(this IDatabaseOptionsBuilder builder)
        {
            if (builder.DatabaseOptions.ConnectionType == ConnectionType.MongoDB)
            {
                builder.Services.AddSingleton<IPageRepository, PageMongoRepository>();
                builder.Services.AddSingleton<ISiteManifestRepository, SiteManifestMongoRepository>();
                builder.Services.AddSingleton<ISiteRepository, SiteMongoRepository>();
                builder.Services.AddSingleton<IThemeRepository, ThemeMongoRepository>();
                builder.Services.AddSingleton<ISiteRouteRepository, SiteRouteMongoRepository>();
                builder.Services.AddSingleton<IPageTemplateRepository, PageTemplateMongoRepository>();
                builder.Services.AddSingleton<IPageVersionRepository, PageVersionMongoRepository>();
            }


            return builder;
        }

        public static void AddCMS(
          this IServiceCollection services,
          IConfiguration configuration)
        {
            var cmsOptions = configuration.GetSection("CMSOptions").Get<CMSOptions>();
            var databaseOptions = configuration.GetSection("DatabaseOptions").Get<DatabaseOptions>();

            services.AddSingleton<IThemeProvider, ThemeProvider>();
            services.AddSingleton<ISiteProvider, SiteProvider>();
            services.AddSingleton<IPageProvider, PageProvider>();
            services.AddSingleton<ISiteRouteProvider, SiteRouteProvider>();

            services.AddScoped<ISiteRequestAccessor, SiteRequestAccessor>();

            services.AddTransient<IPageService, PageService>();
            
            services.AddScoped<CheckingSiteRequestMiddleware>();
            services.AddScoped<CheckingPageRequestMiddleware>();
            services.AddScoped<ResolvingRequestMiddleware>();
            services.AddScoped<CheckingGoogleMetadataRequestMiddleware>();
            services.AddScoped<CheckingPageVersionRequestMiddleware>();
            services.AddScoped<CombiningPageRequestMiddleware>();
            services.AddScoped<CheckingResponseCachingMiddleware>();
            services.AddScoped<CMSTransformer>();

            services.AddTransient<ITagHelperComponent, GoogleMetadataTagHelper>();

            var allScannedAssemblies = GetModuleAssemblies(cmsOptions).ToList();
            allScannedAssemblies.ForEach(a =>
            {
                var moduleRegistration = ModuleUtils.GetModuleRegistration(a);
                if(moduleRegistration != null)
                {
                    services.AddSingleton(moduleRegistration);
                    moduleRegistration.Register(services, configuration, databaseOptions);
                }
            });
        }

        public static PageConventionCollection AddLetPortalCMS(this PageConventionCollection conventions, IConfiguration configuration)
        {
            if(configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var cmsOptions = configuration.GetSection("CMSOptions").Get<CMSOptions>();
            conventions.Add(new ThemePageRouteModelConvention(cmsOptions.DefaultTheme, "/Index", model =>
            {
                // Use the route specified in MapPageRoute for outbound routing.
                foreach (var selector in model.Selectors)
                {
                    selector.AttributeRouteModel.SuppressLinkGeneration = true;
                }

                model.Selectors.Add(new SelectorModel
                {
                    AttributeRouteModel = new AttributeRouteModel
                    {
                        Template = "{level1?}"
                    }
                });
                model.Selectors.Add(new SelectorModel
                {
                    AttributeRouteModel = new AttributeRouteModel
                    {
                        Template = "{level1}/{level2}"
                    }
                });
                model.Selectors.Add(new SelectorModel
                {
                    AttributeRouteModel = new AttributeRouteModel
                    {
                        Template = "{level1}/{level2}/{level3}"
                    }
                });
                model.Selectors.Add(new SelectorModel
                {
                    AttributeRouteModel = new AttributeRouteModel
                    {
                        Template = "{level1}/{level2}/{level3}/{level4}"
                    }
                });
                model.Selectors.Add(new SelectorModel
                {
                    AttributeRouteModel = new AttributeRouteModel
                    {
                        Template = "{level1}/{level2}/{level3}/{level4}/{level5}"
                    }
                });
                model.Selectors.Add(new SelectorModel
                {
                    AttributeRouteModel = new AttributeRouteModel
                    {
                        Template = "{level1}/{level2}/{level3}/{level4}/{level5}/{level6}"
                    }
                });
            }));

            return conventions;
        }

        public static void UseLetPortalCMS(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<CheckingSiteRequestMiddleware>();
            builder.UseMiddleware<CheckingPageRequestMiddleware>();
            builder.UseMiddleware<ResolvingRequestMiddleware>();
            builder.UseMiddleware<CheckingGoogleMetadataRequestMiddleware>();
            builder.UseMiddleware<CheckingPageVersionRequestMiddleware>();
            builder.UseMiddleware<CombiningPageRequestMiddleware>();
            builder.UseMiddleware<CheckingResponseCachingMiddleware>();
        }

        private static IEnumerable<Assembly> GetModuleAssemblies(CMSOptions cmsOptions)
        {
            // Because .NET doesn't load the dll which isn't using anywhere in this assembly
            // So we need to add all .dll that stay in the same folder

            var loadedAssemblies = AssemblyLoadContext.Default.Assemblies;
            var loadedPaths = loadedAssemblies.Select(a => a.Location).ToArray();

            var referencedPaths = Directory.GetFiles(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName, "*.dll");
            var toLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToList();

            toLoad.ForEach(path => 
            {
                try
                {
                    AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
                }
                catch
                {

                }
            });

            return AssemblyLoadContext.Default
                                .Assemblies
                                .Where(a => cmsOptions.Modules.Contains(a.GetName().Name)).ToList();

        }
    }
}
