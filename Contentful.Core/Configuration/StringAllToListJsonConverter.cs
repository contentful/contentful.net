using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Configuration
{
    public class StringAllToListJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(List<string>);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var jToken = JToken.Load(reader);

            if ((jToken.Type == JTokenType.String) && jToken.ToString() == "all")
            {
                return new List<string>()
               {
                   "all"
               };
            }

            return new List<string>((jToken as JArray).Values<string>());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var list = value as List<string>;

            if(list == null)
            {
                return;
            }

            JToken token = JToken.FromObject(value);

            if (list.Count == 1 && list[0] == "all")
            {
                writer.WriteValue("all");
            }else
            {
                token.WriteTo(writer);
            }
        }
    }
}
