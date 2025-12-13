using System;
using System.Collections.Generic;
using System.Reflection;
using LetPortal.Core.Extensions;
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

        public static IDictionary<string, MongoIndexAttribute> GetMongoIndexAttributes(this Type type)
        {
            var dic = new Dictionary<string, MongoIndexAttribute>();
            foreach (var propertyInfo in type.GetProperties())
            {
                var mongoIndex = propertyInfo.GetCustomAttribute<MongoIndexAttribute>();
                if(mongoIndex != null)
                {
                    dic.Add(mongoIndex.Name.ToCamelCase(), mongoIndex);
                }
            }

            return dic;
        }
    }
}
