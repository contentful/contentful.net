using Contentful.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contentful.Core.Configuration
{
    public class AssetJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Asset);


        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) {
                return null;
            }

            var asset = new Asset();

            var jObject = JObject.Load(reader);

            asset.Title = jObject.SelectToken("$.fields.title")?.ToString();
            asset.Description = jObject.SelectToken("$.fields.description")?.ToString();
            asset.File = jObject.SelectToken("$.fields.file")?.ToObject<File>();
            asset.SystemProperties = jObject.SelectToken("$.sys")?.ToObject<SystemProperties>();

            return asset;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
