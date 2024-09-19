using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;

namespace Contentful.Core.Configuration
{
    /// <summary>
    /// JsonConverter for converting <see cref="Contentful.Core.Models.Management.UiExtension"/>.
    /// </summary>
    public class ExtensionJsonConverter : JsonConverter
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver
            {
                NamingStrategy =
                {
                    OverrideSpecifiedNames = false
                }
            },
        };

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">The type to convert to.</param>
        public override bool CanConvert(Type objectType) => objectType == typeof(UiExtension);

        /// <summary>
        /// Gets a value indicating whether this JsonConverter can write JSON.
        /// </summary>
        public override bool CanWrite => true;

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

            var jObject = JObject.Load(reader);
            var extensionProperties = jObject.SelectToken("$.extension");

            var extension = new UiExtension();

            extension.SystemProperties = jObject["sys"].ToObject<SystemProperties>();
            extension.Src = extensionProperties["src"]?.ToString();
            extension.Name = extensionProperties["name"]?.ToString();
            extension.Sidebar = extensionProperties["sidebar"]?.Value<bool>() ?? false;
            extension.FieldTypes = extensionProperties["fieldTypes"]?.Values<dynamic>()?.Select(c => c.type.ToString())?.Cast<string>().ToList();
            extension.SrcDoc = extensionProperties["srcdoc"]?.ToString();
            extension.Parameters = extensionProperties["parameters"]?.ToObject<UiExtensionParametersLists>();
            return extension;
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var extension = value as UiExtension;

            if (extension == null)
            {
                return;
            }
            var extensionStructure = new
            {
                extension
            };

            var jObject = JObject.FromObject(extensionStructure, JsonSerializer.Create(JsonSerializerSettings));

            serializer.Serialize(writer, jObject);
        }
    }
}
