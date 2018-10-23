using Contentful.Core.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Represents a membership of an <see cref="Organization"/>.
    /// </summary>
    public class OrganizationMembership : IContentfulResource
    {
        /// <summary>
        /// Common system managed metadata properties.
        /// </summary>
        [JsonProperty("sys")]
        public SystemProperties SystemProperties { get; set; }

        /// <summary>
        /// Whether or not the requesting user is an administrator of the current <see cref="Space"/> .
        /// </summary>
        public string Role { get; set; }
    }
}
