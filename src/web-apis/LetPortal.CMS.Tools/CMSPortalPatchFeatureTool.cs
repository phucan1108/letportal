using System;
using System.Collections.Generic;
using System.Reflection;
using LetPortal.CMS.Tools.Configurations;
using LetPortal.Core.Tools;
using LetPortal.Core.Utils;
using LetPortal.Identity;
using LetPortal.Portal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LetPortal.CMS.Tools
{
    public class CMSPortalPatchFeatureTool : IPortalPatchFeatureTool
    {
        public string PatchName => "cms";

        public IEnumerable<IPortalVersion> GetAllVersions(IServiceProvider serviceProvider)
        {
            return ReflectionUtil.GetAllInstances<IPortalVersion>(Assembly.GetExecutingAssembly(), serviceProvider);
        }

        public void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration.GetSection("CMSToolOptions").Get<CMSToolOptions>());
        }
    }
}
