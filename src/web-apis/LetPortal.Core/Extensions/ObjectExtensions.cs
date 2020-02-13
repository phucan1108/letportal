using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LetPortal.Core.Utils;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;

namespace LetPortal.Core.Extensions
{
    public static class ObjectExtensions
    {
        private static readonly MethodInfo CloneMethod = typeof(object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);

        public static bool IsPrimitive(this Type type)
        {
            if (type == typeof(String))
            {
                return true;
            }

            return (type.IsValueType & type.IsPrimitive);
        }

        public static object Copy(this object originalObject)
        {
            return InternalCopy(originalObject, new Dictionary<object, object>(new ReferenceEqualityComparer()));
        }
        private static object InternalCopy(object originalObject, IDictionary<object, object> visited)
        {
            if (originalObject == null)
            {
                return null;
            }

            var typeToReflect = originalObject.GetType();
            if (IsPrimitive(typeToReflect))
            {
                return originalObject;
            }

            if (visited.ContainsKey(originalObject))
            {
                return visited[originalObject];
            }

            if (typeof(Delegate).IsAssignableFrom(typeToReflect))
            {
                return null;
            }

            var cloneObject = CloneMethod.Invoke(originalObject, null);
            if (typeToReflect.IsArray)
            {
                var arrayType = typeToReflect.GetElementType();
                if (IsPrimitive(arrayType) == false)
                {
                    var clonedArray = (Array)cloneObject;
                    clonedArray.ForEach((array, indices) => array.SetValue(InternalCopy(clonedArray.GetValue(indices), visited), indices));
                }

            }
            visited.Add(originalObject, cloneObject);
            CopyFields(originalObject, visited, cloneObject, typeToReflect);
            RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);
            return cloneObject;
        }

        private static void RecursiveCopyBaseTypePrivateFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect)
        {
            if (typeToReflect.BaseType != null)
            {
                RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.BaseType);
                CopyFields(originalObject, visited, cloneObject, typeToReflect.BaseType, BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate);
            }
        }

        private static void CopyFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy, Func<FieldInfo, bool> filter = null)
        {
            foreach (var fieldInfo in typeToReflect.GetFields(bindingFlags))
            {
                if (filter != null && filter(fieldInfo) == false)
                {
                    continue;
                }

                if (IsPrimitive(fieldInfo.FieldType))
                {
                    continue;
                }

                var originalFieldValue = fieldInfo.GetValue(originalObject);
                var clonedFieldValue = InternalCopy(originalFieldValue, visited);
                fieldInfo.SetValue(cloneObject, clonedFieldValue);
            }
        }
        public static T Copy<T>(this T original)
        {
            return (T)Copy((object)original);
        }

        public static string ToJson(this object obj)
        {
            return ConvertUtil.SerializeObject(obj);
        }

        public static BsonType GetBsonType(this string value)
        {
            if (DateTime.TryParse(value, null, System.Globalization.DateTimeStyles.RoundtripKind, out _))
            {
                return BsonType.DateTime;
            }

            if (ObjectId.TryParse(value, out _))
            {
                return BsonType.ObjectId;
            }

            if (value.First() == '[' && value.Last() == ']')
            {
                return BsonType.Array;
            }

            if (value.First() == '{' && value.Last() == '}')
            {
                return BsonType.Document;
            }

            if (int.TryParse(value, out _))
            {
                return BsonType.Int32;
            }

            if (long.TryParse(value, out _))
            {
                return BsonType.Int64;
            }

            if (double.TryParse(value, out _))
            {
                return BsonType.Double;
            }

            if (decimal.TryParse(value, out _))
            {
                return BsonType.Decimal128;
            }

            return BsonType.String;
        }

        public static BsonType GetBsonType(this JToken jtoken)
        {
            switch (jtoken.Type)
            {
                case JTokenType.Array:
                    return BsonType.Array;
                case JTokenType.Boolean:
                    return BsonType.Boolean;
                case JTokenType.Date:
                    return BsonType.DateTime;
                case JTokenType.Float:
                    return BsonType.Double;
                case JTokenType.Integer:
                    return BsonType.Int32;
                case JTokenType.Object:
                    return BsonType.Document;
                default:
                    return jtoken.Value<string>().GetBsonType();
            }
        }

        public static object GetValue(this JToken jToken)
        {
            var bsonType = jToken.GetBsonType();
            switch (bsonType)
            {
                case BsonType.Int32:
                    return jToken.ToObject<int>();
                case BsonType.Boolean:
                    return jToken.ToObject<bool>();
                case BsonType.DateTime:
                    return jToken.ToObject<DateTime>();
                case BsonType.Double:
                    return jToken.ToObject<decimal>();
                case BsonType.Array:
                    return jToken.ToList();
                case BsonType.ObjectId:
                    return ObjectId.Parse(jToken.ToString());
                default:
                    return jToken.ToString();
            }
        }
    }

    public class ReferenceEqualityComparer : EqualityComparer<Object>
    {
        public override bool Equals(object x, object y)
        {
            return ReferenceEquals(x, y);
        }
        public override int GetHashCode(object obj)
        {
            if (obj == null)
            {
                return 0;
            }

            return obj.GetHashCode();
        }
    }
}
