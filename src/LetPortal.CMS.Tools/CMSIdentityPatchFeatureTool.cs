using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using LetPortal.Core.Utils;
using LetPortal.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LetPortal.CMS.Tools
{
    public class CMSIdentityPatchFeatureTool : IIdentityPatchFeatureTool
    {
        public string PatchName => "cms";

        public IEnumerable<IIdentityVersion> GetAllVersions(IServiceProvider serviceProvider)
        {
            return ReflectionUtil.GetAllInstances<IIdentityVersion>(Assembly.GetExecutingAssembly(), serviceProvider);
        }

        public void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            // Do nothing
        }
    }
}
