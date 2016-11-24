using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Represents the response of a webhook health check.
    /// </summary>
    public class WebHookHealthResponse : IContentfulResource
    {
        /// <summary>
        /// Common system managed metadata properties.
        /// </summary>
        [JsonProperty("sys", NullValueHandling = NullValueHandling.Ignore)]
        public SystemProperties SystemProperties { get; set; }

        public int TotalCalls { get; set; }
        public int TotalHealthy { get; set; }
    }
}
