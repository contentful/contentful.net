using Newtonsoft.Json;
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

        /// <summary>
        /// The taxonomy references for the content type.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<TaxonomyReference> Taxonomy { get; set; }
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

    /// <summary>
    /// Represents a reference to a taxonomy concept or concept scheme in content type metadata.
    /// </summary>
    public class TaxonomyReference
    {
        /// <summary>
        /// Whether the taxonomy concept or concept scheme is required.
        /// </summary>
        [JsonProperty("required")]
        public string Required { get; set; }

        /// <summary>
        /// The reference to the taxonomy concept or concept scheme.
        /// </summary>
        public ReferenceProperties Sys { get; set; }
    }
}

