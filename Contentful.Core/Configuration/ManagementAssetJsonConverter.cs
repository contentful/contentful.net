using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Configuration
{
    /// <summary>
    /// JsonConverter for converting <see cref="Contentful.Core.Models.Management.ManagementAsset"/>.
    /// </summary>
    public class ManagementAssetJsonConverter : JsonConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">The type to convert to.</param>
        public override bool CanConvert(Type objectType) => objectType == typeof(ManagementAsset);

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

            var asset = new ManagementAsset();

            var jObject = JObject.Load(reader);

            asset.Title = jObject.SelectToken("$.fields.title")?.ToObject<Dictionary<string, string>>();
            asset.Description = jObject.SelectToken("$.fields.description")?.ToObject<Dictionary<string, string>>();
            asset.Files = jObject.SelectToken("$.fields.file")?.ToObject<Dictionary<string, File>>();
            asset.SystemProperties = jObject.SelectToken("$.sys")?.ToObject<SystemProperties>();

            return asset;
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                return;

            var asset = value as ManagementAsset;

            serializer.Serialize(writer, new { sys = asset.SystemProperties,
                fields = new { title = asset.Title, description = asset.Description, file = asset.Files } });
        }
    }
}
