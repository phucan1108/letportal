using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LetPortal.Core.Common
{
    public class BsonConverter : JsonConverter
    {
        private readonly List<FormatBsonField> formatBsonFields;

        public BsonConverter(
            List<FormatBsonField> formatBsonFields = null)
        {
            this.formatBsonFields = formatBsonFields ?? new List<FormatBsonField>();
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            // We have two problems with Bson -> json
            // 1. ObjectId, in case we use StrictMode, Bson will return "_id" : { "$oid": "abc" }
            // 2. Date, in case we use StrictMode, Bson will return "date": { "$date": "ISO Format" }
            var replacedDic = new Dictionary<string, string>();
            var children = jObject.Children();
            foreach (JProperty child in children)
            {
                if (child.Value.Type == JTokenType.Object)
                {
                    if ((child.Value as JObject).Properties().Any(a => (a as JProperty).Name == "$oid"))
                    {
                        var objectIdStr = (child.Value as JObject).Properties().First().Value.ToString();
                        replacedDic.Add(child.Name, objectIdStr);
                    }
                    else if ((child.Value as JObject).Properties().Any(a => (a as JProperty).Name == "$date"))
                    {
                        var dateStr = (child.Value as JObject).Properties().First().Value.ToString();
                        var longEpochTime = long.Parse(dateStr);
                        var date = DateTimeOffset.FromUnixTimeMilliseconds(longEpochTime);
                        var format = formatBsonFields.FirstOrDefault(a => a.BsonFieldName == child.Name);
                        if (format != null)
                        {
                            replacedDic.Add(child.Name, string.Format(format.Format, date));
                        }
                        else
                        {
                            replacedDic.Add(child.Name, date.ToString("o"));
                        }
                    }
                }
            }

            foreach (var kvp in replacedDic)
            {
                jObject.Remove(kvp.Key);
                jObject.Add(kvp.Key == "_id" ? "id" : kvp.Key, JToken.FromObject(kvp.Value));
            }

            children = jObject.Children();
            replacedDic = new Dictionary<string, string>();
            if (formatBsonFields?.Count > 0)
            {
                foreach (JProperty child in children)
                {
                    var formatField = formatBsonFields.FirstOrDefault(a => a.BsonFieldName == child.Name);
                    if (formatField != null)
                    {
                        try
                        {
                            switch (child.Value.Type)
                            {
                                case JTokenType.Integer:
                                    replacedDic.Add(child.Name, string.Format(formatField.Format, long.Parse(child.Value.ToString())));
                                    break;
                                case JTokenType.Float:
                                    replacedDic.Add(child.Name, string.Format(formatField.Format, float.Parse(child.Value.ToString())));
                                    break;
                                case JTokenType.Date:
                                    replacedDic.Add(child.Name, string.Format(formatField.Format, DateTime.Parse(child.Value.ToString())));
                                    break;
                                case JTokenType.String:
                                    replacedDic.Add(child.Name, string.Format(formatField.Format, child.Value.ToString()));
                                    break;
                                default:
                                    break;
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            }

            foreach (var kvp in replacedDic)
            {
                jObject.Remove(kvp.Key);
                jObject.Add(kvp.Key == "_id" ? "id" : kvp.Key, JToken.FromObject(kvp.Value));
            }

            return JsonConvert.DeserializeObject(jObject.ToString(Formatting.Indented), objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class FormatBsonField
    {
        public string BsonFieldName { get; set; }

        public string Format { get; set; }
    }
}
