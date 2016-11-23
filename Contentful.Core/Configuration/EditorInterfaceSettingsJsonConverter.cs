using Contentful.Core.Models.Management;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Configuration
{
    public class EditorInterfaceControlJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(EditorInterfaceControl);

        public override bool CanWrite => false;


        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var jsonObject = JObject.Load(reader);
            var settings = new EditorInterfaceControlSettings();

            var editorInterfaceControl = new EditorInterfaceControl();
            editorInterfaceControl.FieldId = jsonObject["fieldId"]?.ToString();
            editorInterfaceControl.WidgetId = jsonObject["widgetId"]?.ToString();

            if(jsonObject["settings"] == null)
            {
                return editorInterfaceControl;
            }

            if (jsonObject["widgetId"]?.ToString() == "boolean")
            {
                var boolSettings = new BooleanEditorInterfaceControlSettings();
                boolSettings.FalseLabel = jsonObject["settings"]?["falseLabel"]?.ToString();
                boolSettings.TrueLabel = jsonObject["settings"]?["trueLabel"]?.ToString();
                settings = boolSettings;
            }

            if (jsonObject["widgetId"]?.ToString() == "rating")
            {
                var ratingSettings = new RatingEditorInterfaceControlSettings();

                var stars = 0;

                if(!int.TryParse(jsonObject["settings"]?["stars"]?.ToString(), out stars))
                {
                    stars = 5;
                }

                ratingSettings.NumberOfStars = stars;

                settings = ratingSettings;
            }

            if (jsonObject["widgetId"]?.ToString() == "datePicker")
            {
                var dateSettings = new DatePickerEditorInterfaceControlSettings();

                dateSettings.ClockFormat = jsonObject["settings"]?["ampm"]?.ToString();
                dateSettings.DateFormat = (EditorInterfaceDateFormat)Enum.Parse(typeof(EditorInterfaceDateFormat), jsonObject["settings"]?["format"]?.ToString(), true);

                settings = dateSettings;
            }

            settings.HelpText = jsonObject["settings"]?["helpText"]?.ToString();

            editorInterfaceControl.Settings = settings;

            return editorInterfaceControl;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
