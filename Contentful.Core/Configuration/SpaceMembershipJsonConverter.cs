using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contentful.Core.Configuration
{
    /// <summary>
    /// JsonConverter for converting <see cref="Contentful.Core.Models.Management.SpaceMembership"/>.
    /// </summary>
    public class SpaceMembershipJsonConverter : JsonConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">The type to convert to.</param>
        public override bool CanConvert(Type objectType) => objectType == typeof(SpaceMembership);

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

            var spaceMembership = new SpaceMembership();

            spaceMembership.SystemProperties = jObject["sys"]?.ToObject<SystemProperties>();
            spaceMembership.Admin = jObject.Value<bool>("admin");
            spaceMembership.Roles = jObject.SelectTokens("$.roles..id")?.Values<string>().ToList();

            return spaceMembership;
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
            var spaceMembership = value as SpaceMembership;

            if(spaceMembership == null)
            {
                return;
            }

            var jArray = new JArray();

            if (spaceMembership.Roles != null)
            {
                foreach (var role in spaceMembership.Roles)
                {
                    jArray.Add(new JObject(
                        new JProperty("type", "Link"),
                        new JProperty("linkType", "Role"),
                        new JProperty("id", role.ToString())
                        ));
                }
            }

            var jObject = new JObject(
               new JProperty("admin", spaceMembership.Admin),
               new JProperty("roles", jArray)
               );

            jObject.WriteTo(writer);
        }
    }
}
