using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Configuration
{
    public class ManagementAssetJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(ManagementAsset);

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

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                return;

            var asset = value as ManagementAsset;

            serializer.Serialize(writer, new { sys = asset.SystemProperties, fields = new { title = asset.Title, description = asset.Description, file = asset.Files } });
        }
    }
}
