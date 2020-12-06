using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace LetPortal.Core.Utils
{
    public class ConvertUtil
    {
        public static object MoveValueBetweenTwoObjects(object sourceObject, object targetObject)
        {
            var sourceObjectType = sourceObject.GetType();
            IEnumerable<PropertyInfo> itemProperties;

            itemProperties = sourceObjectType.GetProperties()
                .Where(a => a.GetGetMethod() != null && !a.GetGetMethod().IsVirtual)
                .Where(c => c.GetSetMethod() != null && !c.GetSetMethod().IsVirtual);

            targetObject.GetType().GetProperties()
                .Where(a => a.GetGetMethod() != null && a.GetSetMethod() != null)
                .ToList()
                .ForEach(a =>
                {
                    var relevantProperty =
                        itemProperties.FirstOrDefault(b =>
                            b.Name == a.Name && b.PropertyType.IsAssignableFrom(a.PropertyType));

                    if (relevantProperty != null)
                    {
                        a.SetValue(targetObject, relevantProperty.GetValue(sourceObject));
                    }
                });

            return targetObject;
        }

        public static string SerializeObject(object serializingObject, bool allowCamel = false)
        {
            if (serializingObject != null)
            {
                if (allowCamel)
                {
                    var contractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    };

                    return JsonConvert.SerializeObject(serializingObject, new JsonSerializerSettings
                    {
                        ContractResolver = contractResolver,
                        Formatting = Formatting.Indented
                    });
                }
                return JsonConvert.SerializeObject(serializingObject);
            }
            else
            {
                return string.Empty;
            }
        }

        public static T DeserializeObject<T>(string deserializingObject)
        {
            return JsonConvert.DeserializeObject<T>(deserializingObject);
        }

        public static object DeserializeObject(string deserializingObject, Type type)
        {
            return JsonConvert.DeserializeObject(deserializingObject, type);
        }

        public static dynamic GetOneInArray(dynamic array)
        {
            if (array.Type == JTokenType.Array)
            {
                var jArray = ((JArray)array);
                return jArray?.Count == 1 ? jArray[0].ToObject<dynamic>() : array;
            }

            return array;
        }

        public static object DeserializeObjectToObject(string deserializingObject)
        {
            return JsonConvert.DeserializeObject(deserializingObject);
        }

        public static IDictionary<string, object> DeserializeObjectToDic(string deserializingObject)
        {
            var jObject = JObject.Parse(deserializingObject);
            var dics = new Dictionary<string, object>();
            foreach (var elem in jObject)
            {
                dics.Add(elem.Key, elem.Value.Value<string>());
            }

            return dics;
        } 
    }
}
