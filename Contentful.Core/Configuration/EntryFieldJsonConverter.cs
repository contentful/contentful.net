using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Contentful.Core.Models;

namespace Contentful.Core.Configuration
{
    public class EntryFieldJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => true;
        private bool _canRead = true;

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
            var fields = jObject.SelectToken("$.fields");

            //Important to set to false to make sure we don't try to use the JsonConverter again inside of ToObject
            _canRead = false;
            var returnObject = fields?.ToObject(objectType);
            _canRead = true;
            return returnObject;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
