using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Configuration
{
    public class SnapshotJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Snapshot);

        public override bool CanWrite => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var jObject = JObject.Load(reader);
            var fields = jObject.SelectToken("$.snapshot.fields");

            var snapshot = new Snapshot();

            snapshot.SystemProperties = jObject["sys"].ToObject<SystemProperties>();
            snapshot.Fields = fields.ToObject<Dictionary<string, Dictionary<string, dynamic>>>();

            return snapshot;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
