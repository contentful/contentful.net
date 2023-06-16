using Contentful.Core.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Represents how a way to organize how fields are displayed in Compose page editor
    /// </summary>
    public class EditorInterfaceGroupControl
    {
        /// <summary>
        /// The group id of the field that this control represents.
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// The id of the type of widget to use for this group.
        /// </summary>
        public string WidgetId { get; set; }

        /// <summary>
        /// The widget namespace.
        /// </summary>
        public string WidgetNamespace { get; set; }

        /// <summary>
        /// Represents the settings for this group control
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public EditorInterfaceGroupControlSettings EditorInterfaceGroupControlSettings { get; set; }
    }

    /// <summary>
    /// Represents custom settings for a EditorInterfaceGroupControl widget.
    /// </summary>
    public class EditorInterfaceGroupControlSettings
    {
        /// <summary>
        /// The help text that should accompany this field.
        /// </summary>
        public string HelpText { get; set; }

        /// <summary>
        /// Whether the field set is collapsed at page load.
        /// </summary>
        public bool CollapsedByDefault { get; set; }
    }
}
