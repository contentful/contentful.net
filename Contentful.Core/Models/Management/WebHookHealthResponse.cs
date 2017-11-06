using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Represents the response of a webhook health check.
    /// </summary>
    public class WebhookHealthResponse : IContentfulResource
    {
        /// <summary>
        /// Common system managed metadata properties.
        /// </summary>
        [JsonProperty("sys", NullValueHandling = NullValueHandling.Ignore)]
        public SystemProperties SystemProperties { get; set; }

        /// <summary>
        /// The total number of webhook calls made.
        /// </summary>
        public int TotalCalls { get; set; }

        /// <summary>
        /// The total number of healthy calls.
        /// </summary>
        public int TotalHealthy { get; set; }
    }
}
