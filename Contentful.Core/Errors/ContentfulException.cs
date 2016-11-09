using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contentful.Core.Models;

namespace Contentful.Core.Errors
{
    /// <summary>
    /// Represents errors that occurr when calling the Contentful APIs.
    /// </summary>
    public class ContentfulException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContentfulException"/> class.
        /// </summary>
        /// <param name="statusCode">The http status code of the exception.</param>
        /// <param name="message">The message of the exception.</param>
        public ContentfulException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// The http status code of the exception.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Common system managed metadata properties.
        /// </summary>
        public SystemProperties SystemProperties { get; set; }

        /// <summary>
        /// The details of the exception.
        /// </summary>
        public ErrorDetails ErrorDetails { get; set; }

        /// <summary>
        /// The ID of the request to the Contentful API.
        /// </summary>
        public string RequestId { get; set; }
    }
}
