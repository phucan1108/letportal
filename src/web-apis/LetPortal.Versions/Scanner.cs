using System.Collections.Generic;
using System.Reflection;
using LetPortal.Core.Utils;

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
