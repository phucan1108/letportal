using System;
using System.Collections.Generic;
using System.Reflection;
using LetPortal.Core.Utils;
using LetPortal.Identity;
using LetPortal.Portal;

namespace LetPortal.Versions
{
    public class Scanner
    {
        public static IEnumerable<IPortalVersion> GetAllPortalVersions(IServiceProvider serviceProvider)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();

            return ReflectionUtil.GetAllInstances<IPortalVersion>(currentAssembly, serviceProvider);
        }

        public static IEnumerable<IIdentityVersion> GetAllIdentityVersions(IServiceProvider serviceProvider)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();

            return ReflectionUtil.GetAllInstances<IIdentityVersion>(currentAssembly, serviceProvider);
        }
    }
}
