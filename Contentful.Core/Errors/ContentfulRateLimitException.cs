using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Errors
{
    /// <summary>
    /// Represents errors that occurr when a call hit the rate limit of the API.
    /// </summary>
    public class ContentfulRateLimitException : ContentfulException
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Contentful.Core.Errors.ContentfulRateLimitException"/>.
        /// </summary>
        /// <param name="message">The message of the exception.</param>
        public ContentfulRateLimitException(string message) : base(429, message)
        {
        }

        /// <summary>
        /// The number of seconds until the next request can be made to the API.
        /// </summary>
        public int SecondsUntilNextRequest { get; set; }
    }
}
