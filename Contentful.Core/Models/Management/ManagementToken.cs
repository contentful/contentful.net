using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Represents a personal access token for the management API.
    /// </summary>
    public class ManagementToken : IContentfulResource
    {
        /// <summary>
        /// Common system managed metadata properties.
        /// </summary>
        [JsonProperty("sys")]
        public SystemProperties SystemProperties { get; set; }

        /// <summary>
        /// The name of the token
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A DateTime representing when the token was revoked. If it's not revoked the value will be null.
        /// </summary>
        public DateTime? RevokedAt { get; set; }

        /// <summary>
        /// The scopes this access token is valid for.
        /// </summary>
        public List<string> Scopes { get; set; }

        /// <summary>
        /// The access token.
        /// </summary>
        public string Token { get; set; }
    }
}
