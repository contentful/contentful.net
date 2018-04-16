using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contentful.Core.Errors;

namespace Contentful.Core.Configuration
{
    /// <summary>
    /// Represents a set of options to configure a <see cref="ContentfulClient"/>.
    /// </summary>
    public class ContentfulOptions
    {
        /// <summary>
        /// The api key used when communicating with the Contentful delivery API.
        /// </summary>
        public string DeliveryApiKey { get; set; }

        /// <summary>
        /// The api key used when communicating with the Contentful preview API.
        /// <remarks>
        /// To use the preview API the <see cref="UsePreviewApi"/> property must be set to true.
        /// </remarks>
        /// </summary>
        public string PreviewApiKey { get; set; }

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
        /// If set the client will evaluate the class to serialize into and only serialize the parts that are part of the class structure.
        /// </summary>
        public bool ResolveEntriesSelectively { get; set; }

        /// <summary>
        /// Sets the default number of times to retry after hitting a <see cref="Contentful.Core.Errors.ContentfulRateLimitException"/>.
        /// 0 means that no retries will be made. Maximum is 10.
        /// </summary>
        public int MaxNumberOfRateLimitRetries { get; set; }

        /// <summary>
        /// Gets or sets the space environment to use with this client.
        /// </summary>
        public string Environment { get; set; }
    }
}
