using System;
using System.Linq;
using System.Reflection;
using LetPortal.CMS.Core.Abstractions;

namespace LetPortal.CMS.Core.Utils
{
    public static class ModuleUtils
    {
        public static IModuleRegistration GetModuleRegistration(Assembly target)
        {
            var deliveredType = target.GetTypes().FirstOrDefault(a => a.GetInterface(typeof(IModuleRegistration).Name) != null);
            if(deliveredType != null)
            {
                return Activator.CreateInstance(deliveredType) as IModuleRegistration;
            }
            return default;            
        }
    }
}
