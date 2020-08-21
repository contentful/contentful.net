using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Represents an webhook configuration in a <see cref="Space"/>.
    /// </summary>
    public class Webhook : IContentfulResource
    {
        /// <summary>
        /// Common system managed metadata properties.
        /// </summary>
        [JsonProperty("sys", NullValueHandling = NullValueHandling.Ignore)]
        public SystemProperties SystemProperties { get; set; }

        /// <summary>
        /// The url to call for this webhook. 
        /// Bear in mind that **Private IPs**, **Localhost**, **hostnames without top-level domain** and **URLs that resolve to redirects or localhost** are not allowed.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The basic authentication username to pass with the webhook.
        /// </summary>
        public string HttpBasicUsername { get; set; }

        /// <summary>
        /// The basic authentication password to pass with the webhook.
        /// </summary>
        public string HttpBasicPassword { get; set; }

        /// <summary>
        /// The name of the web hook.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The custom http headers to pass with the webhook.
        /// </summary>
        public List<WebhookHeader> Headers { get; set; }

        /// <summary>
        /// The topics that trigger this webhook.
        /// </summary>
        public List<string> Topics { get; set; }

        /// <summary>
        /// The filters applied to this webhook.
        /// </summary>
        public List<IConstraint> Filters { get; set; }

        /// <summary>
        /// The transformation applied to this webhook.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public WebhookTransformation Transformation { get; set; }
    }
}
