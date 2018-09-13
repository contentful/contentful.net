using Contentful.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contentful.Core.Configuration
{
    /// <summary>
    /// JsonConverter for converting Contentful assets into a simpler <see cref="Asset"/> structure.
    /// </summary>
    public class ContentJsonConverter : JsonConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">The type to convert to.</param>
        public override bool CanConvert(Type objectType) => objectType == typeof(IContent);

        /// <summary>
        /// Gets a value indicating whether this JsonConverter can write JSON.
        /// </summary>
        public override bool CanWrite => false;

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
            if (reader.TokenType == JsonToken.Null) {
                return null;
            }
            
            var jObject = JObject.Load(reader);
            if (jObject.TryGetValue("$ref", out var refId))
            {
                return serializer.ReferenceResolver.ResolveReference(serializer, ((JValue)refId).Value.ToString());
            }
            var type = jObject.Value<string>("nodeType");
            var serializationType = jObject.Value<string>("$type");
            if (!string.IsNullOrEmpty(serializationType))
            {
                var typeinfo = Type.GetType(serializationType);
                return jObject.ToObject(typeinfo, serializer);
            }

            if (type.StartsWith("heading"))
            {
                var sizeString = type.Split("-".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)[1];
                var size = 1;
                int.TryParse(sizeString, out size);

                var heading = jObject.ToObject<Heading>(serializer);
                heading.HeadingSize = size;
                return heading;
            }

            switch (type)
            {
                case "paragraph":
                    return jObject.ToObject<Paragraph>(serializer);
                case "mark":
                    return jObject.ToObject<Mark>();
                case "text":
                    return jObject.ToObject<Text>();
                case "hyperlink":
                    return jObject.ToObject<Hyperlink>(serializer);
                case "Asset":
                    return jObject.ToObject<Asset>(serializer);
                case "quote":
                    return jObject.ToObject<Quote>(serializer);
                case "ordered-list":
                case "unordered-list":
                    return jObject.ToObject<List>(serializer);
                case "list-item":
                    return jObject.ToObject<ListItem>(serializer);
                case "hr":
                    return jObject.ToObject<HorizontalRuler>();
                case "entry-hyperlink":
                case "embedded-entry-block":
                    return jObject.ToObject<EntryHyperlink>(serializer);
                default:
                    break;
            }

            return new Mark();
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// **NOTE: This method is not implemented and will throw an exception.**
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
