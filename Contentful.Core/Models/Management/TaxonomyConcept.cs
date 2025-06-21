using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Contentful.Core.Models.Management;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Represents a taxonomy concept in Contentful.
    /// </summary>
    public class TaxonomyConcept : IContentfulResource
    {
        /// <summary>
        /// Common system managed metadata properties.
        /// </summary>
        [JsonProperty("sys")]
        public SystemProperties SystemProperties { get; set; }

        /// <summary>
        /// The URI of the concept.
        /// </summary>
        [JsonProperty("uri")]
        public string Uri { get; set; }

        /// <summary>
        /// The preferred label of the concept in different locales.
        /// </summary>
        [JsonProperty("prefLabel")]
        public Dictionary<string, string> PrefLabel { get; set; }

        /// <summary>
        /// Alternative labels of the concept in different locales.
        /// </summary>
        [JsonProperty("altLabels")]
        public Dictionary<string, string[]> AltLabels { get; set; }

        /// <summary>
        /// Hidden labels of the concept in different locales.
        /// </summary>
        [JsonProperty("hiddenLabels")]
        public Dictionary<string, string[]> HiddenLabels { get; set; }

        /// <summary>
        /// Notations associated with the concept.
        /// </summary>
        [JsonProperty("notations")]
        public string[] Notations { get; set; }

        /// <summary>
        /// Notes about the concept in different locales.
        /// </summary>
        [JsonProperty("note")]
        public Dictionary<string, string> Note { get; set; }

        /// <summary>
        /// Change notes about the concept in different locales.
        /// </summary>
        [JsonProperty("changeNote")]
        public Dictionary<string, string> ChangeNote { get; set; }

        /// <summary>
        /// Definition of the concept in different locales.
        /// </summary>
        [JsonProperty("definition")]
        public Dictionary<string, string> Definition { get; set; }

        /// <summary>
        /// Editorial notes about the concept in different locales.
        /// </summary>
        [JsonProperty("editorialNote")]
        public Dictionary<string, string> EditorialNote { get; set; }

        /// <summary>
        /// Examples of the concept in different locales.
        /// </summary>
        [JsonProperty("example")]
        public Dictionary<string, string> Example { get; set; }

        /// <summary>
        /// History notes about the concept in different locales.
        /// </summary>
        [JsonProperty("historyNote")]
        public Dictionary<string, string> HistoryNote { get; set; }

        /// <summary>
        /// Scope notes about the concept in different locales.
        /// </summary>
        [JsonProperty("scopeNote")]
        public Dictionary<string, string> ScopeNote { get; set; }

        /// <summary>
        /// Broader concepts that this concept belongs to.
        /// </summary>
        [JsonProperty("broader")]
        public Reference[] Broader { get; set; }

        /// <summary>
        /// Related concepts.
        /// </summary>
        [JsonProperty("related")]
        public Reference[] Related { get; set; }
    }
} 