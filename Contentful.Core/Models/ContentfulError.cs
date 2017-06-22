using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models
{
    /// <summary>
    /// Encapsulates an error returned from the Contentful API.
    /// </summary>
    public class ContentfulError : IContentfulResource
    {
        /// <summary>
        /// Common system managed metadata properties.
        /// </summary>
        [JsonProperty("sys")]
        public SystemProperties SystemProperties { get; set; }

        /// <summary>
        /// Encapsulates the details of the error.
        /// </summary>
        public ContentfulErrorDetails Details { get; set; }
    }

    /// <summary>
    /// Encapsulates detailed information about an error returned by the Contentful API.
    /// </summary>
    public class ContentfulErrorDetails
    {
        /// <summary>
        /// The type of error.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The type of link causing the error.
        /// </summary>
        public string LinkType { get; set; }

        /// <summary>
        /// The id of the resource causing issues.
        /// </summary>
        public string Id { get; set; }
    }
}
