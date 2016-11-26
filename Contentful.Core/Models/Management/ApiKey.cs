using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Represents an api key for the Contentful Content Delivery API.
    /// </summary>
    public class ApiKey : IContentfulResource
    {
        /// <summary>
        /// Common system managed metadata properties.
        /// </summary>
        [JsonProperty("sys", NullValueHandling = NullValueHandling.Ignore)]
        public SystemProperties SystemProperties { get; set; }

        /// <summary>
        /// The name of the API key.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of the API key.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The access token for the API key.
        /// </summary>
        public string AccessToken { get; set; }
    }
}
