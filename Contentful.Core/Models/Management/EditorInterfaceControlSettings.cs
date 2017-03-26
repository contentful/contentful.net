using Newtonsoft.Json;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Represents custom settings for a widget in a <see cref="EditorInterfaceControl"/>.
    /// </summary>
    public class EditorInterfaceControlSettings
    {
        /// <summary>
        /// The help text that should accompany this field.
        /// </summary>
        public string HelpText { get; set; }
    }

    /// <summary>
    /// Represents custom settings for a boolean widget.
    /// </summary>
    public class BooleanEditorInterfaceControlSettings : EditorInterfaceControlSettings
    {
        /// <summary>
        /// The text to accompany the true choice of the widget.
        /// </summary>
        public string TrueLabel { get; set; }

        /// <summary>
        /// The text to accompany the false choice of the widget.
        /// </summary>
        public string FalseLabel { get; set; }
    }

    /// <summary>
    /// Represents custom settings for the star widget.
    /// </summary>
    public class RatingEditorInterfaceControlSettings : EditorInterfaceControlSettings
    {
        /// <summary>
        /// The number of stars to show in the widget.
        /// </summary>
        [JsonProperty("stars")]
        public int NumberOfStars { get; set; }
    }

    /// <summary>
    /// Represents custom settings for a datepicker widget.
    /// </summary>
    public class DatePickerEditorInterfaceControlSettings : EditorInterfaceControlSettings
    {
        /// <summary>
        /// The format for the date in the widget.
        /// </summary>
        [JsonProperty("format")]
        public EditorInterfaceDateFormat DateFormat { get; set; }

        /// <summary>
        /// The format for the clock in the widget. Allowed values are "12" or "24".
        /// </summary>
        [JsonProperty("ampm")]
        public string ClockFormat { get; set; }
    }

    /// <summary>
    /// Enumeration of available formats for a datetime editorinterface.
    /// </summary>
    public enum EditorInterfaceDateFormat
    {
        /// <summary>
        /// Time and date.
        /// </summary>
        timeZ,
        /// <summary>
        /// Time only.
        /// </summary>
        time,
        /// <summary>
        /// Date only.
        /// </summary>
        dateonly
    }

}
