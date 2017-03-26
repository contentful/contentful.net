using Contentful.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Configuration
{
    /// <summary>
    /// JsonConverter to convert a list of string to a Contentful representation.
    /// </summary>
    public class StringAllToListJsonConverter : JsonConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">The type to convert to.</param>
        public override bool CanConvert(Type objectType) => objectType == typeof(List<string>);

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The reader to use.</param>
        /// <param name="objectType">The object type to serialize into.</param>
        /// <param name="existingValue">The current value of the property.</param>
        /// <param name="serializer">The serializer to use.</param>
        /// <returns>The deserialized <see cref="Asset"/>.</returns>
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

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
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
