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
    public class SpaceMembershipJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(SpaceMembership);

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
