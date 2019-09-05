using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;

namespace LetPortal.Core.Utils
{
    public static class ReflectionUtil
    {
        public static IEnumerable<T> GetAllInstances<T>(this Assembly assembly)
        {
            var allDeliveriedTypes = assembly.GetTypes().Where(a => typeof(T).IsAssignableFrom(a) && !a.IsInterface && !a.IsAbstract).Select(a => a);

            var instances = from type in allDeliveriedTypes
                            select (T)Activator.CreateInstance(type);

            return instances;
        }
    }
}