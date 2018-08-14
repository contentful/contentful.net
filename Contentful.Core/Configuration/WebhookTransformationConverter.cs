using Contentful.Core.Models.Management;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Configuration
{
    /// <summary>
    /// JsonConverter for converting to and from a webhook transformation HTTP method.
    /// </summary>
    public class WebhookTransformationMethodConverter : JsonConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">The type to convert to.</param>
        public override bool CanConvert(Type objectType) => objectType == typeof(HttpMethods);

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The reader to use.</param>
        /// <param name="objectType">The object type to serialize into.</param>
        /// <param name="existingValue">The current value of the property.</param>
        /// <param name="serializer">The serializer to use.</param>
        /// <returns>The deserialized HttpMethod.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return HttpMethods.POST;
            }

            var jObject = JObject.Load(reader);
            var stringmethod = jObject.Value<string>();

            return Enum.Parse(typeof(HttpMethods), stringmethod);
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var successfulParsing = Enum.TryParse<HttpMethods>(value?.ToString(), out var method);

            if (successfulParsing == false)
            {
                return;
            }

            serializer.Serialize(writer, method);
        }
    }

    /// <summary>
    /// JsonConverter for converting to and from a webhook transformation HTTP method.
    /// </summary>
    public class WebhookTransformationContenttypeConverter : JsonConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">The type to convert to.</param>
        public override bool CanConvert(Type objectType) => objectType == typeof(TransformationContentTypes);

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The reader to use.</param>
        /// <param name="objectType">The object type to serialize into.</param>
        /// <param name="existingValue">The current value of the property.</param>
        /// <param name="serializer">The serializer to use.</param>
        /// <returns>The deserialized content type.</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return TransformationContentTypes.ContentfulManagementPlusJson;
            }

            var jObject = JObject.Load(reader);
            var stringcontenttype = jObject.Value<string>();

            return Enum.Parse(typeof(TransformationContentTypes), stringcontenttype);
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var successfulParsing = Enum.TryParse<TransformationContentTypes>(value?.ToString(), out var contenttype);

            if (successfulParsing == false)
            {
                return;
            }

            serializer.Serialize(writer, contenttype);
        }
    }
}
