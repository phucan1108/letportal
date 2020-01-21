using LetPortal.Core.Utils;
using System.Collections.Generic;
using System.Reflection;

namespace LetPortal.Versions
{
    public class Scanner
    {
        public static IEnumerable<IPortalVersion> GetAllPortalVersions()
        {
            var currentAssembly = Assembly.GetExecutingAssembly();

            return ReflectionUtil.GetAllInstances<IPortalVersion>(currentAssembly);
        }

        public static IEnumerable<IIdentityVersion> GetAllIdentityVersions()
        {
            var currentAssembly = Assembly.GetExecutingAssembly();

            return ReflectionUtil.GetAllInstances<IIdentityVersion>(currentAssembly);
        }
    }
}
