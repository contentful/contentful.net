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
    public class AssetJsonConverter : JsonConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">The type to convert to.</param>
        public override bool CanConvert(Type objectType) => objectType == typeof(Asset);

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

            var asset = new Asset();

            var jObject = JObject.Load(reader);
            if (jObject.TryGetValue("$ref", out var refId))
            {
                return serializer.ReferenceResolver.ResolveReference(serializer, ((JValue) refId).Value.ToString());
            }

            asset.SystemProperties = jObject.SelectToken("$.sys")?.ToObject<SystemProperties>();

            if (!string.IsNullOrEmpty(asset.SystemProperties.Locale))
            {
                asset.Title = jObject.SelectToken("$.fields.title")?.ToString();
                asset.TitleLocalized = new Dictionary<string, string>();
                asset.TitleLocalized.Add(asset.SystemProperties.Locale, asset.Title);
                asset.Description = jObject.SelectToken("$.fields.description")?.ToString();
                asset.DescriptionLocalized = new Dictionary<string, string>();
                asset.DescriptionLocalized.Add(asset.SystemProperties.Locale, asset.Description);
                asset.File = jObject.SelectToken("$.fields.file")?.ToObject<File>();
                asset.FilesLocalized = new Dictionary<string, File>();
                asset.FilesLocalized.Add(asset.SystemProperties.Locale, asset.File);
            }
            else
            {
                asset.TitleLocalized = jObject.SelectToken("$.fields.title")?.ToObject<Dictionary<string, string>>();
                asset.DescriptionLocalized = jObject.SelectToken("$.fields.description")?.ToObject<Dictionary<string, string>>();
                asset.FilesLocalized = jObject.SelectToken("$.fields.file")?.ToObject<Dictionary<string, File>>();
            }

            var existing = serializer.ReferenceResolver.ResolveReference(serializer, asset.SystemProperties.Id) as Asset;

            if (existing != null)
                return existing;

            if (!serializer.ReferenceResolver.IsReferenced(serializer, asset))
            {
                serializer.ReferenceResolver.AddReference(serializer, asset.SystemProperties.Id, asset);
            }
            return asset;
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
