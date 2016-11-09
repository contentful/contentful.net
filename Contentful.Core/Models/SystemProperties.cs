using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contentful.Core.Models
{
    /// <summary>
    /// Encapsulates system managed metadata returned by the Contentful APIs.
    /// </summary>
    public class SystemProperties
    {
        /// <summary>
        /// The unique identifier of the resource.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The type of link. Will be null for non link types.
        /// </summary>
        public string LinkType { get; set; }

        /// <summary>
        /// The type of the resource.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The published version of the resource. Will be null for non-versioned types.
        /// </summary>
        public int? Revision { get; set; }

        /// <summary>
        /// The date and time the resource was created. Will be null when not applicable, e.g. for arrays.
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// The date and time the resource was last updated. Will be null when not applicable or when the resource has never been updated.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// The date and time the resource was deleted. This field will only be present for <seealso cref="SyncResult"/> deleted items.
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// The locale of the resource. Will only have a value for <seealso cref="Asset"/> and <seealso cref="Entry"/> resource types.
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// The <seealso cref="ContentType"/> of the resource. Only applicable for <seealso cref="Entry"/> resource types.
        /// </summary>
        public ContentType ContentType { get; set; }

        /// <summary>
        /// The <seealso cref="Space"/> of the resource. Only applicable for <seealso cref="Entry"/>, <seealso cref="Asset"/> and <seealso cref="ContentType"/> resource types.
        /// </summary>
        public Space Space { get; set; }
    }
}
