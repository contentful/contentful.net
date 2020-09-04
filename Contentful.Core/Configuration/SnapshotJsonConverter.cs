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
    /// JsonConverter for converting <see cref="Contentful.Core.Models.Management.Snapshot"/>.
    /// </summary>
    public class SnapshotJsonConverter : JsonConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">The type to convert to.</param>
        public override bool CanConvert(Type objectType) => objectType == typeof(Snapshot);

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
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var jObject = JObject.Load(reader);
            var fields = jObject.SelectToken("$.snapshot.fields");

            var snapshot = new Snapshot();

            snapshot.SystemProperties = jObject["sys"].ToObject<SystemProperties>();
            snapshot.Fields = fields.ToObject<Dictionary<string, Dictionary<string, dynamic>>>();
            snapshot.EntityProperties = jObject.SelectToken("$.snapshot.sys").ToObject<SystemProperties>();
            return snapshot;
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
