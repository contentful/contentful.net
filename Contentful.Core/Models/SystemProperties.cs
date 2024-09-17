﻿using Contentful.Core.Models.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contentful.Core.Models
{
    /// <summary>
    /// Encapsulates system managed metadata returned by the Contentful APIs.
    /// </summary>
    public class SystemProperties : BaseSystemProperties
    {
        /// <summary>
        /// The published version of the resource. Will be null for non-versioned types.
        /// </summary>
        public int? Revision { get; set; }

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
        /// The number of times the resource has been published.
        /// </summary>
        public int? PublishedCounter { get; set; }

        /// <summary>
        /// The published version of the resource. Will only be present for management API calls.
        /// </summary>
        public int? PublishedVersion { get; set; }

        /// <summary>
        /// The user that published the resource. Will only be present for management API calls.
        /// </summary>
        public User PublishedBy { get; set; }

        /// <summary>
        /// When the resource was last published. Will only be present for management API calls.
        /// </summary>
        public DateTime? PublishedAt { get; set; }

        /// <summary>
        /// The number of times the resource has been published. Will only be present for management API calls.
        /// </summary>
        public int? PublishCounter { get; set; }

        /// <summary>
        /// When the resource was first published. Will only be present for management API calls.
        /// </summary>
        public DateTime? FirstPublishedAt { get; set; }

        /// <summary>
        /// The date and time the resource was archived. Will only be present for management API calls.
        /// </summary>
        public DateTime? ArchivedAt { get; set; }

        /// <summary>
        /// The version that is currently archived. Will only be present for management API calls.
        /// </summary>
        public int? ArchivedVersion { get; set; }

        /// <summary>
        /// The link to the user that last archived this content. Will only be present for management API call.
        /// </summary>
        public User ArchivedBy { get; set; }

        /// <summary>
        /// The organization the resource links to. Will only be present for certain management API calls.
        /// </summary>
        public Organization Organization { get; set; }

        /// <summary>
        /// The usage period the resource links to. Will only be present for certain management API calls.
        /// </summary>
        public UsagePeriod UsagePeriod { get; set; }

        /// <summary>
        /// The link to the status that the current object had. Used only for resources that have a status.
        /// </summary>
        public Status Status { get; set; }
    }
}


