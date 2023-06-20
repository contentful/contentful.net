using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contentful.Core.Models.Management;
using Newtonsoft.Json;

namespace Contentful.Core.Models
{
    /// <summary>
    /// Represents a single content type.
    /// </summary>
    public class ContentType : IContentfulResource
    {
        /// <summary>
        /// Common system managed metadata properties.
        /// </summary>
        [JsonProperty("sys", NullValueHandling = NullValueHandling.Ignore)]
        public SystemProperties SystemProperties { get; set; }

        /// <summary>
        /// Metadata properties for the content type.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ContentTypeMetadata Metadata { get; set; }

        /// <summary>
        /// The name of the content type.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of the content type.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The ID of the main field used for display.
        /// </summary>
        public string DisplayField { get; set; }

        /// <summary>
        /// List of <seealso cref="Field"/> this content type contains.
        /// </summary>
        public List<Field> Fields { get; set; }
    }
}
