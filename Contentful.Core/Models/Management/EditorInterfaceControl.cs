using Contentful.Core.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Represents how a specific field of a <see cref="ContentType"/> should be represented visually.
    /// </summary>
    [JsonConverter(typeof(EditorInterfaceControlJsonConverter))]
    public class EditorInterfaceControl
    {
        /// <summary>
        /// The id of the field that this control represents.
        /// </summary>
        public string FieldId { get; set; }

        /// <summary>
        /// The id of the type of widget to use for this field. See also <seealso cref="SystemWidgetIds"/> for a list of system widget ids.
        /// </summary>
        public string WidgetId { get; set; }

        /// <summary>
        /// Represents custom settings for a widget.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public EditorInterfaceControlSettings Settings { get; set; }
    }
}
