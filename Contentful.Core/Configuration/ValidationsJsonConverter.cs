using Contentful.Core.Models.Management;
using Contentful.Core.Search;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            var jsonObject = JObject.Load(reader);
            JToken jToken;

            if (jsonObject.TryGetValue("size", out jToken))
            {
                return new SizeValidator(
                    jToken["min"] != null ? new int?(int.Parse(jToken["min"].ToString())) : null,
                    jToken["max"] != null ? new int?(int.Parse(jToken["max"].ToString())) : null,
                    jsonObject["message"]?.ToString());
            }

            if (jsonObject.TryGetValue("range", out jToken))
            {
                return new RangeValidator(
                    jToken["min"] != null ? new int?(int.Parse(jToken["min"].ToString())) : null,
                    jToken["max"] != null ? new int?(int.Parse(jToken["max"].ToString())) : null,
                    jsonObject["message"]?.ToString());
            }

            if (jsonObject.TryGetValue("in", out jToken))
            {
                return new InValuesValidator(jToken.Value<List<string>>(), jsonObject["message"]?.ToString());
            }

            if (jsonObject.TryGetValue("linkMimetypeGroup", out jToken))
            {
                return new MimeTypeValidator((MimeTypeRestriction)Enum.Parse(typeof(MimeTypeRestriction),jToken.Value<string>(), true),
                    jsonObject["message"]?.ToString());
            }

            if (jsonObject.TryGetValue("linkContentType", out jToken))
            {
                return new LinkContentTypeValidator(jToken.Value<List<string>>(), jsonObject["message"]?.ToString());
            }

            if (jsonObject.TryGetValue("regexp", out jToken))
            {
                return new RegexValidator(jToken["pattern"].ToString(), jToken["flags"].ToString(), jsonObject["message"]?.ToString());
            }

            if (jsonObject.TryGetValue("unique", out jToken))
            {
                return new UniqueValidator();
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, (value as IFieldValidator).CreateValidator());
        }
    }
}
