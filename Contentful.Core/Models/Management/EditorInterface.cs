using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Represents the editor interface of a <see cref="ContentType"/>.
    /// </summary>
    public class EditorInterface : IContentfulResource
    {
        /// <summary>
        /// Common system managed metadata properties.
        /// </summary>
        [JsonProperty("sys", NullValueHandling = NullValueHandling.Ignore)]
        public SystemProperties SystemProperties { get; set; }

        /// <summary>
        /// List of <see cref="EditorInterfaceControl"/> representing the type of editor interface for each field of a <see cref="ContentType"/>.
        /// </summary>
        public List<EditorInterfaceControl> Controls { get; set; }

        /// <summary>
        /// List of <see cref="EditorInterfaceGroupControl"/> representing the group controls for a number of fields of a <see cref="ContentType"/>.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<EditorInterfaceGroupControl> GroupControls { get; set; }

        /// <summary>
        /// List of <see cref="EditorLayout"/> describing the hierarchical organization of fields and groups of a <see cref="ContentType"/>.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<EditorLayoutGroup> EditorLayout { get; set; }
    }
}
