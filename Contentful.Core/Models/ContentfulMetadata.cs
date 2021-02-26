using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models
{
    /// <summary>
    /// Represents a Contentful metadata object for this entry or asset.
    /// </summary>
    public class ContentfulMetadata
    {
        /// <summary>
        /// The tags associated with this entry or asset.
        /// </summary>
        public List<string> Tags { get; set; }
    }
}
