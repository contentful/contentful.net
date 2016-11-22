using Contentful.Core.Models.Management;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Configuration
{
    public class ValidationsJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType is IFieldValidator;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, (value as IFieldValidator).CreateValidator());
        }
    }
}
