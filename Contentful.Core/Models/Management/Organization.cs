using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Represents a single organization.
    /// </summary>
    public class Organization : IContentfulResource
    {
        /// <summary>
        /// The name of the organization.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Common system managed metadata properties.
        /// </summary>
        [JsonProperty("sys")]
        public SystemProperties SystemProperties { get; set; }
    }
}
