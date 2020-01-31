using System;
using System.Reflection;
using LetPortal.Core.Persistences.Attributes;

namespace LetPortal.Core.Persistences
{
    public static class PersistenceExtensions
    {
        public static EntityCollectionAttribute GetEntityCollectionAttribute(this Type type)
        {
            return type.GetCustomAttribute<EntityCollectionAttribute>(true);
        }

        public static string GetEntityCollectionName(this Type type)
        {
            return type.GetCustomAttribute<EntityCollectionAttribute>(true).Name;
        }
    }
}
