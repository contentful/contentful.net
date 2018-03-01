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

        /// <summary>
        /// A link to the preview api key coupled with this api key.
        /// </summary>
        [JsonProperty("preview_api_key", NullValueHandling = NullValueHandling.Ignore)]
        public ApiKey PreviewApiKey { get; set; }

        /// <summary>
        /// Links to all the environments to which this API key has access.
        /// </summary>
        public List<ContentfulEnvironment> Environments { get; set; }
    }
}
