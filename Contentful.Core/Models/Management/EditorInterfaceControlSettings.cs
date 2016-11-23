using Newtonsoft.Json;

namespace Contentful.Core.Models.Management
{

    public class EditorInterfaceControlSettings
    {
        public string HelpText { get; set; }
    }

    public class BooleanEditorInterfaceControlSettings : EditorInterfaceControlSettings
    {
        public string TrueLabel { get; set; }
        public string FalseLabel { get; set; }
    }

    public class RatingEditorInterfaceControlSettings : EditorInterfaceControlSettings
    {
        [JsonProperty("stars")]
        public int NumberOfStars { get; set; }
    }

    public class DatePickerEditorInterfaceControlSettings : EditorInterfaceControlSettings
    {
        [JsonProperty("format")]
        public EditorInterfaceDateFormat DateFormat { get; set; }
        [JsonProperty("ampm")]
        public string ClockFormat { get; set; }
    }

    public enum EditorInterfaceDateFormat
    {
        timeZ,
        time,
        dateonly
    }

}
