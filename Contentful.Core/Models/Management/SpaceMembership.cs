using Contentful.Core.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Represents a membership of a <see cref="Space"/>.
    /// </summary>
    [JsonConverter(typeof(SpaceMembershipJsonConverter))]
    public class SpaceMembership : IContentfulResource
    {
        /// <summary>
        /// Common system managed metadata properties.
        /// </summary>
        [JsonProperty("sys")]
        public SystemProperties SystemProperties { get; set; }

        /// <summary>
        /// Whether or not the requesting user is an administrator of the current <see cref="Space"/> .
        /// </summary>
        public bool Admin { get; set; }

        /// <summary>
        /// A list of id's of roles the user is member of for the current <see cref="Space"/>.
        /// </summary>
        public List<string> Roles { get; set; }

        /// <summary>
        /// The <see cref="User"/> that belongs to this membership.
        /// </summary>
        public User User { get; set; }
    }
}
