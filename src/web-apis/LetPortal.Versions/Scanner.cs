using LetPortal.Core.Utils;
using LetPortal.Core.Versions;
using System.Collections.Generic;
using System.Reflection;

namespace LetPortal.Versions
{
    public class Scanner
    {
        public static IEnumerable<IVersion> GetAllVersions()
        {
            var currentAssembly = Assembly.GetExecutingAssembly();

            return ReflectionUtil.GetAllInstances<IVersion>(currentAssembly);
        }
    }
}
