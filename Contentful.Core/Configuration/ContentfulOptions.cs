using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contentful.Core.Configuration
{
    /// <summary>
    /// Represents a set of options to configure a <see cref="ContentfulClient"/>.
    /// </summary>
    public class ContentfulOptions
    {
        /// <summary>
        /// The api key used when communicating with the Contentful delivery or preview API.
        /// <remarks>If you specify the <see cref="UsePreviewApi"/> option, 
        /// you need to use the preview API key to call the Contentful API.</remarks>
        /// </summary>
        public string DeliveryApiKey { get; set; }

        /// <summary>
        /// The api key used when communicating with the Contentful management API.
        /// </summary>
        public string ManagementApiKey { get; set; }

        /// <summary>
        /// The ID of the space that you wish to get or manipulate content for.
        /// </summary>
        public string SpaceId { get; set; }

        /// <summary>
        /// Whether or not to use the Preview API for requests. 
        /// If this is set to true the preview API key needs to be used for <see cref="DeliveryApiKey"/>.
        /// </summary>
        public bool UsePreviewApi { get; set; }

        /// <summary>
        /// Sets the default number of times to retry after hitting a <see cref="ContentfulRateLimitException"/>.
        /// 0 means that no retries will be made. Maxiumum is 10.
        /// </summary>
        public int MaxNumberOfRateLimitRetries { get; set; }
    }
}
