using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Represents a status object.
    /// </summary>
    public class Status
    {
        /// <summary>
        /// The type of object. Always returns "Link".
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The type of link. Normally one of <see cref="SystemLinkTypes"/>.
        /// </summary>
        public string LinkType { get; set; }

        /// <summary>
        /// The id of the status, representing the current status.
        /// </summary>
        public string Id { get; set; }
    }
}
