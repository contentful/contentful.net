using Contentful.Core.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Represents the snapshot of an entry at a given time in the past.
    /// </summary>
    [JsonConverter(typeof(SnapshotJsonConverter))]
    public class Snapshot : IContentfulResource
    {
        /// <summary>
        /// Common system managed metadata properties.
        /// </summary>
        [JsonProperty("sys")]
        public SystemProperties SystemProperties { get; set; }

        /// <summary>
        /// The systemproperties of the snapshotted entity.
        /// </summary>
        public SystemProperties EntityProperties { get; set; }

        /// <summary>
        /// The fields of the <see cref="Entry{T}"/> in all languages.
        /// </summary>
        public Dictionary<string, Dictionary<string, dynamic>> Fields { get; set; }
    }
}
