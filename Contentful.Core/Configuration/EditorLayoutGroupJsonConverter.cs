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
    /// JsonConverter for converting <see cref="Contentful.Core.Models.Management.EditorInterface"/>.
    /// </summary>
    public class EditorInterfaceControlJsonConverter : JsonConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">The type to convert to.</param>
        public override bool CanConvert(Type objectType) => objectType == typeof(EditorInterfaceControl);

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
                return null;

            var jsonObject = JObject.Load(reader);
            var settings = new EditorInterfaceControlSettings();

            var editorInterfaceControl = new EditorInterfaceControl()
            {
                FieldId = jsonObject["fieldId"]?.ToString(),
                WidgetId = jsonObject["widgetId"]?.ToString()
            };

            if (jsonObject["settings"] == null)
            {
                return editorInterfaceControl;
            }

            if (jsonObject["widgetId"]?.ToString() == "boolean")
            {
                var boolSettings = new BooleanEditorInterfaceControlSettings()
                {
                    FalseLabel = jsonObject["settings"]?["falseLabel"]?.ToString(),
                    TrueLabel = jsonObject["settings"]?["trueLabel"]?.ToString()
                };
                settings = boolSettings;
            }

            if (jsonObject["widgetId"]?.ToString() == "rating")
            {
                var ratingSettings = new RatingEditorInterfaceControlSettings();

                var stars = 0;

                if (!int.TryParse(jsonObject["settings"]?["stars"]?.ToString(), out stars))
                {
                    stars = 5;
                }

                ratingSettings.NumberOfStars = stars;

                settings = ratingSettings;
            }

            if (jsonObject["widgetId"]?.ToString() == "datePicker")
            {
                var dateSettings = new DatePickerEditorInterfaceControlSettings()
                {
                    ClockFormat = jsonObject["settings"]?["ampm"]?.ToString(),
                    DateFormat = (EditorInterfaceDateFormat)Enum.Parse(typeof(EditorInterfaceDateFormat), jsonObject["settings"]?["format"]?.ToString(), true)
                };
                settings = dateSettings;
            }

            settings.HelpText = jsonObject["settings"]?["helpText"]?.ToString();

            editorInterfaceControl.Settings = settings;

            return editorInterfaceControl;
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
