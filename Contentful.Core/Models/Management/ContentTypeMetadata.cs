﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Represents a Contentful metadata object for this content type.
    /// </summary>
    public class ContentTypeMetadata
    {
        /// <summary>
        /// The metadata annotations
        /// </summary>
        public ContentTypeMetadataAnnotation Annotations { get; set; }
    }

    public class ContentTypeMetadataAnnotation
    {
        /// <summary>
        /// Annotations for the content type.
        /// </summary>
        [JsonProperty("ContentType")]
        public List<Reference> ContentType { get; set; } = new List<Reference>();

        /// <summary>
        /// Annotations for the content type fields.
        /// </summary>
        [JsonProperty("ContentTypeField", NullValueHandling = NullValueHandling.Ignore)]
        public dynamic ContentTypeField { get; set; }
    }
}

