using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Contentful.Core.Models.Management;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Represents a taxonomy concept scheme in Contentful.
    /// </summary>
    public class TaxonomyConceptScheme : IContentfulResource
    {
        /// <summary>
        /// Common system managed metadata properties.
        /// </summary>
        [JsonProperty("sys")]
        public SystemProperties SystemProperties { get; set; }

        /// <summary>
        /// The URI of the concept scheme.
        /// </summary>
        [JsonProperty("uri")]
        public string Uri { get; set; }

        /// <summary>
        /// The preferred label of the concept scheme in different locales.
        /// </summary>
        [JsonProperty("prefLabel")]
        public Dictionary<string, string> PrefLabel { get; set; }

        /// <summary>
        /// Definition of the concept scheme in different locales.
        /// </summary>
        [JsonProperty("definition")]
        public Dictionary<string, string> Definition { get; set; }

        /// <summary>
        /// Top concepts in the scheme.
        /// </summary>
        [JsonProperty("topConcepts")]
        public List<ConceptReference> TopConcepts { get; set; }

        /// <summary>
        /// All concepts in the scheme.
        /// </summary>
        [JsonProperty("concepts")]
        public List<ConceptReference> Concepts { get; set; }

        /// <summary>
        /// Total number of concepts in the scheme.
        /// </summary>
        [JsonProperty("totalConcepts")]
        public int TotalConcepts { get; set; }
    }

    /// <summary>
    /// Represents a reference to a concept within a concept scheme.
    /// </summary>
    public class ConceptReference
    {
        /// <summary>
        /// The ID of the concept.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
    }
} 