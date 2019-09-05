using LetPortal.Core.Utils;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace LetPortal.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToJson(this object obj)
        {
            return ConvertUtil.SerializeObject(obj);
        }

        public static BsonType GetBsonType(this string value)
        {
            if(DateTime.TryParse(value, null, System.Globalization.DateTimeStyles.RoundtripKind, out _))
            {
                return BsonType.DateTime;
            }

            if(ObjectId.TryParse(value, out _))
            {
                return BsonType.ObjectId;
            }

            if(value.First() == '[' && value.Last() == ']')
            {
                return BsonType.Array;
            }

            if(value.First() == '{' && value.Last() == '}')
            {
                return BsonType.Document;
            }

            if(int.TryParse(value, out _))
            {
                return BsonType.Int32;
            }

            if(long.TryParse(value, out _))
            {
                return BsonType.Int64;
            }

            if(double.TryParse(value, out _))
            {
                return BsonType.Double;
            }

            if(decimal.TryParse(value, out _))
            {
                return BsonType.Decimal128;
            }

            return BsonType.String;
        }

        public static BsonType GetBsonType(this JToken jtoken)
        {
            switch(jtoken.Type)
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
            switch(bsonType)
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
}
