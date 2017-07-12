using Contentful.Core.Models.Management;
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
        /// The current version of the resource. Will only be present for management API calls. 
        /// </summary>
        public int? Version { get; set; }

        /// <summary>
        /// The date and time the resource was created. Will be null when not applicable, e.g. for arrays.
        /// </summary>
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// The link to the user that created this content. Will only be present for management API call.
        /// </summary>
        public User CreatedBy { get; set; }

        /// <summary>
        /// The date and time the resource was last updated. Will be null when not applicable or when the resource has never been updated.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// The link to the user that last updated this content. Will only be present for management API call.
        /// </summary>
        public User UpdatedBy { get; set; }

        /// <summary>
        /// The date and time the resource was deleted. This field will only be present for <seealso cref="SyncResult"/> deleted items.
        /// </summary>
        public DateTime? DeletedAt { get; set; }

        /// <summary>
        /// The locale of the resource. Will only have a value for <seealso cref="Asset"/> and <seealso cref="Entry{T}"/> resource types.
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// The <seealso cref="ContentType"/> of the resource. Only applicable for <seealso cref="Entry{T}"/> resource types.
        /// </summary>
        public ContentType ContentType { get; set; }

        /// <summary>
        /// The <seealso cref="Space"/> of the resource. Only applicable for <seealso cref="Entry{T}"/>, <seealso cref="Asset"/> and <seealso cref="ContentType"/> resource types.
        /// </summary>
        public Space Space { get; set; }

        /// <summary>
        /// The number of times the resource has been published.
        /// </summary>
        public int? PublishedCounter { get; set; }

        /// <summary>
        /// The user that published the resource.
        /// </summary>
        public User PublishedBy { get; set; }

        /// <summary>
        /// When the resource was first published.
        /// </summary>
        public DateTime? FirstPublishedAt { get; set; }
    }
}
