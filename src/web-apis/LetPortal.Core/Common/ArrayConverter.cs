using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LetPortal.Core.Common
{
    public class ArrayConverter : JsonConverter
    {
        private readonly List<FieldFormatCompare> _fieldFormats;

        public ArrayConverter(List<FieldFormatCompare> fieldFormats)
        {
            _fieldFormats = fieldFormats;
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jArray = JArray.Load(reader);
            var resObjects = new List<object>();
            foreach (var jObject in jArray.Children<JObject>())
            {
                // We have two problems with Bson -> json
                // 1. ObjectId, in case we use StrictMode, Bson will return "_id" : { "$oid": "abc" }
                // 2. Date, in case we use StrictMode, Bson will return "date": { "$date": "ISO Format" }
                var replacedDic = new Dictionary<string, string>();
                var children = jObject.Children();
                replacedDic = new Dictionary<string, string>();
                if (_fieldFormats?.Count > 0)
                {
                    foreach (JProperty child in children)
                    {
                        var formatField = _fieldFormats.FirstOrDefault(a => a.FieldName == child.Name);
                        if (formatField != null)
                        {
                            try
                            {
                                switch (child.Value.Type)
                                {
                                    case JTokenType.Integer:
                                        replacedDic.Add(child.Name, string.Format(formatField.FieldFormat, long.Parse(child.Value.ToString())));
                                        break;
                                    case JTokenType.Float:
                                        replacedDic.Add(child.Name, string.Format(formatField.FieldFormat, float.Parse(child.Value.ToString())));
                                        break;
                                    case JTokenType.Date:
                                        replacedDic.Add(child.Name, string.Format(formatField.FieldFormat, DateTime.Parse(child.Value.ToString())));
                                        break;
                                    case JTokenType.String:
                                        replacedDic.Add(child.Name, string.Format(formatField.FieldFormat, child.Value.ToString()));
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

                resObjects.Add(JsonConvert.DeserializeObject(jObject.ToString(Formatting.Indented), objectType));
            }
            return resObjects;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class FieldFormatCompare
    {
        public string FieldName { get; set; }

        public string FieldFormat { get; set; }
    }
}
