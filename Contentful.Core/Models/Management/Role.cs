using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Represents a role with permissions and policies in Contentful.
    /// </summary>
    public class Role : IContentfulResource
    {
        /// <summary>
        /// Common system managed metadata properties.
        /// </summary>
        [JsonProperty("sys", NullValueHandling = NullValueHandling.Ignore)]
        public SystemProperties SystemProperties { get; set; }

        /// <summary>
        /// The name of the role.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of the role.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The list of <see cref="Policy">policies</see> applied to this role.
        /// </summary>
        public List<Policy> Policies { get; set; }

        /// <summary>
        /// The general high level permissions this role has.
        /// </summary>
        public ContentfulPermissions Permissions { get; set; }
    }
}
