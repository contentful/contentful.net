using Contentful.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Configuration
{
    /// <summary>
    /// JsonConverter for converting <see cref="Contentful.Core.Models.File"/>.
    /// </summary>
    public class FileJsonConverter : JsonConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">The type to convert to.</param>
        public override bool CanConvert(Type objectType) => objectType == typeof(File);

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

            var file = new File();
            file.Details = jObject["details"]?.ToObject<FileDetails>();
            file.ContentType = jObject["contentType"]?.ToString();
            file.FileName = jObject["fileName"]?.ToString();
            file.Url = jObject["url"]?.ToString();

            if (jObject["upload"] != null)
            {
                if (jObject["upload"].Type == JTokenType.Object)
                {
                    file.UploadReference = jObject["upload"].ToObject<SystemProperties>();
                }
                else
                {
                    file.UploadUrl = jObject["upload"].ToString();
                }
            }

            return file;
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                return;

            var file = value as File;

            serializer.Serialize(writer, new
            {
                file.Details, 
                file.ContentType,
                file.FileName,
                file.Url,
                Upload = file.UploadReference
            });
        }
    }
}
