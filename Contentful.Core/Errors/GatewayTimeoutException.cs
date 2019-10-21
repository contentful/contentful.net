using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Errors
{
    /// <summary>
    /// Represents errors that occurr when the Contentful API returns 504 Gateway Timeout responses.
    /// </summary>
    public class GatewayTimeoutException : ContentfulException
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Contentful.Core.Errors.GatewayTimeoutException"/>.
        /// </summary>
        public GatewayTimeoutException() : base(429, "Gateway Timeout")
        {
        }
    }
}
