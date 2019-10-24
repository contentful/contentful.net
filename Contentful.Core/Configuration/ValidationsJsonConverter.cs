using Contentful.Core.Extensions;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contentful.Core.Search;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contentful.Core.Configuration
{
    /// <summary>
    /// JsonConverter for converting <see cref="Contentful.Core.Models.Management.IFieldValidator"/>.
    /// </summary>
    public class ValidationsJsonConverter : JsonConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">The type to convert to.</param>
        public override bool CanConvert(Type objectType) => objectType is IFieldValidator;

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
            var jsonObject = JObject.Load(reader);

            if (jsonObject.TryGetValue("size", out var jToken))
            {
                return new SizeValidator(
                    jToken["min"].ToNullableInt(),
                    jToken["max"].ToNullableInt(),
                    jsonObject["message"]?.ToString());
            }


            if (jsonObject.TryGetValue("range", out jToken))
            {
                return new RangeValidator(
                    jToken["min"].ToNullableInt(),
                    jToken["max"].ToNullableInt(),
                    jsonObject["message"]?.ToString());
            }

            if (jsonObject.TryGetValue("in", out jToken))
            {
                return new InValuesValidator(jToken.Values<string>(), jsonObject["message"]?.ToString());
            }

            if (jsonObject.TryGetValue("linkMimetypeGroup", out jToken))
            {
                if(jToken is JValue)
                {
                    //single string value returned for mime type field. This seems to be an inconsistency in the API that needs to be handled.

                    var type = jToken.Value<string>();
                    return new MimeTypeValidator(new[] { (MimeTypeRestriction)Enum.Parse(typeof(MimeTypeRestriction), type, true) },
                    jsonObject["message"]?.ToString());
                }

                var types = jToken.Values<string>();
                return new MimeTypeValidator(types.Select(c => (MimeTypeRestriction)Enum.Parse(typeof(MimeTypeRestriction), c, true)),
                    jsonObject["message"]?.ToString());
            }

            if (jsonObject.TryGetValue("linkContentType", out jToken))
            {
                return new LinkContentTypeValidator(jToken.Values<string>(), jsonObject["message"]?.ToString());
            }

            if (jsonObject.TryGetValue("regexp", out jToken))
            {
                return new RegexValidator(jToken["pattern"]?.ToString(), jToken["flags"]?.ToString(), jsonObject["message"]?.ToString());
            }

            if (jsonObject.TryGetValue("prohibitRegexp", out jToken))
            {
                return new ProhibitRegexValidator(jToken["pattern"]?.ToString(), jToken["flags"]?.ToString(), jsonObject["message"]?.ToString());
            }

            if (jsonObject.TryGetValue("unique", out jToken))
            {
                return new UniqueValidator();
            }

            if (jsonObject.TryGetValue("dateRange", out jToken))
			{
				return new DateRangeValidator(
					jToken["min"]?.ToString(),
					jToken["max"]?.ToString(),
					jsonObject["message"]?.ToString());
			}

            if (jsonObject.TryGetValue("assetFileSize", out jToken))
			{
				return new FileSizeValidator(
					jToken["min"].ToNullableInt(),
					jToken["max"].ToNullableInt(),
					SystemFileSizeUnits.Bytes,
					SystemFileSizeUnits.Bytes,
					jsonObject["message"]?.ToString());
			}

            if (jsonObject.TryGetValue("assetImageDimensions", out jToken))
			{
				int? minWidth = null;
				int? maxWidth = null;
				int? minHeight = null;
				int? maxHeight = null;
				if (jToken["width"] != null)
				{
					var width = jToken["width"];
					minWidth = width["min"].ToNullableInt();
					maxWidth = width["max"].ToNullableInt();
				}
				if (jToken["height"] != null)
				{
					var height = jToken["height"];
					minHeight = height["min"].ToNullableInt();
					maxHeight = height["max"].ToNullableInt();
				}
				return new ImageSizeValidator(minWidth, maxWidth, minHeight, maxHeight, jsonObject["message"]?.ToString());
			}

            if (jsonObject.TryGetValue("nodes", out jToken))
            {
                var validator = new NodesValidator();

                if (jToken["entry-hyperlink"] != null)
                {
                    validator.EntryHyperlink = jToken["entry-hyperlink"].ToObject<IEnumerable<IFieldValidator>>(serializer);
                }
                if (jToken["embedded-entry-block"] != null)
                {
                    validator.EmbeddedEntryBlock = jToken["embedded-entry-block"].ToObject<IEnumerable<IFieldValidator>>(serializer);
                }
                if (jToken["embedded-entry-inline"] != null)
                {
                    validator.EmbeddedEntryInline = jToken["embedded-entry-inline"].ToObject<IEnumerable<IFieldValidator>>(serializer);
                }

                return validator;
            }

            if (jsonObject.TryGetValue("enabledMarks", out jToken))
            {
                var types = jToken.Values<string>();
                return new EnabledMarksValidator(types.Select(c => (EnabledMarkRestrictions)Enum.Parse(typeof(EnabledMarkRestrictions), c, true)),
                    jsonObject["message"]?.ToString());
            }


            if (jsonObject.TryGetValue("enabledNodeTypes", out jToken))
            {
                var types = jToken.Values<string>();
                return new EnabledNodeTypesValidator(types,
                    jsonObject["message"]?.ToString());
            }

            return Activator.CreateInstance(objectType);
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, (value as IFieldValidator).CreateValidator());
        }
    }
}
