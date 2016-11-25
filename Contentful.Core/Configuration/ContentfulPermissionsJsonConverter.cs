using Contentful.Core.Models.Management;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Configuration
{
    public class ContentfulPermissionsJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(ContentfulPermissions);

        public override bool CanWrite => false;

        private bool _canRead = true;

        /// <summary>
        /// Gets a value indicating whether this converter can currently read JSON.
        /// </summary>
        public override bool CanRead
        {
            get
            {
                return _canRead;
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var jObject = JObject.Load(reader);

            if(jObject["ContentModel"]?.ToString() == "all")
            {
                jObject["ContentModel"] = new JArray()
                {
                    "all"
                };
            }
            if (jObject["Settings"]?.ToString() == "all")
            {
                jObject["Settings"] = new JArray()
                {
                    "all"
                };
            }
            if (jObject["ContentDelivery"]?.ToString() == "all")
            {
                jObject["ContentDelivery"] = new JArray()
                {
                    "all"
                };
            }

            //Important to set to false to make sure we don't try to use the JsonConverter again inside of ToObject
            _canRead = false;
            var returnObject = jObject.ToObject<ContentfulPermissions>();
            _canRead = true;
            return returnObject;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
