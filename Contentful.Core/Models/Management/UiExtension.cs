using Contentful.Core.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Encapsulates information about a Contentful Ui Extension
    /// </summary>
    [JsonConverter(typeof(ExtensionJsonConverter))]
    public class UiExtension : IContentfulResource
    {
        /// <summary>
        /// Common system managed metadata properties.
        /// </summary>
        [JsonProperty("sys")]
        public SystemProperties SystemProperties { get; set; }

        /// <summary>
        /// The source URL for html file for the extension.
        /// </summary>
        public string Src { get; set; }

        /// <summary>
        /// String representation of the widget, e.g. inline HTML.
        /// </summary>
        public string SrcDoc { get; set; }

        /// <summary>
        /// The name of the extension
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The field types for which this extension applies.
        /// </summary>
        public List<string> FieldTypes { get; set; }

        /// <summary>
        /// Whether or not this is an extension for the Contentful sidebar.
        /// </summary>
        public bool Sidebar { get; set; }

        /// <summary>
        /// The parameters applicable to this extension.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public UiExtensionParametersLists Parameters { get; set; }
    }

    /// <summary>
    /// Class encapsulating the two lists of extension parameters available.
    /// </summary>
    public class UiExtensionParametersLists
    {
        /// <summary>
        /// The parameters that should be available at installation time of the extension.
        /// </summary>
        [JsonProperty("installation")]
        public List<UiExtensionParameters> InstallationParameters { get; set; }

        /// <summary>
        /// The parameters that should be available at runtime for the extension.
        /// </summary>
        [JsonProperty("instance")]
        public List<UiExtensionParameters> InstanceParameters { get; set; }
    }
}
