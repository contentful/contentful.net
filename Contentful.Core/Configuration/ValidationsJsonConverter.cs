using Contentful.Core.Models.Management;
using Contentful.Core.Search;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Configuration
{
    /// <summary>
    /// JsonConverter for converting <see cref="IFieldValidator"/>.
    /// </summary>
    public class ValidationsJsonConverter : JsonConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">The type to convert to.</param>
        public override bool CanConvert(Type objectType)
        {
            return objectType is IFieldValidator;
        }

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
            var jsonObject = JObject.Load(reader);

            JToken jToken;

            if (jsonObject.TryGetValue("size", out jToken))
            {
                return new SizeValidator(
                    jToken["min"] != null ? new int?(int.Parse(jToken["min"].ToString())) : null,
                    jToken["max"] != null ? new int?(int.Parse(jToken["max"].ToString())) : null,
                    jsonObject["message"]?.ToString());
            }

            if (jsonObject.TryGetValue("range", out jToken))
            {
                return new RangeValidator(
                    jToken["min"] != null ? new int?(int.Parse(jToken["min"].ToString())) : null,
                    jToken["max"] != null ? new int?(int.Parse(jToken["max"].ToString())) : null,
                    jsonObject["message"]?.ToString());
            }

            if (jsonObject.TryGetValue("in", out jToken))
            {
                return new InValuesValidator(jToken.Value<List<string>>(), jsonObject["message"]?.ToString());
            }

            if (jsonObject.TryGetValue("linkMimetypeGroup", out jToken))
            {
                return new MimeTypeValidator((MimeTypeRestriction)Enum.Parse(typeof(MimeTypeRestriction),jToken.Value<string>(), true),
                    jsonObject["message"]?.ToString());
            }

            if (jsonObject.TryGetValue("linkContentType", out jToken))
            {
                return new LinkContentTypeValidator(jToken.Value<List<string>>(), jsonObject["message"]?.ToString());
            }

            if (jsonObject.TryGetValue("regexp", out jToken))
            {
                return new RegexValidator(jToken["pattern"].ToString(), jToken["flags"].ToString(), jsonObject["message"]?.ToString());
            }

            if (jsonObject.TryGetValue("unique", out jToken))
            {
                return new UniqueValidator();
            }

            return null;
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, (value as IFieldValidator).CreateValidator());
        }
    }
}
