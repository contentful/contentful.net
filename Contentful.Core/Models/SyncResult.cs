using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Contentful.Core.Models
{
    /// <summary>
    /// Represents the result of a sync operation.
    /// </summary>
    public class SyncResult
    {
        /// <summary>
        /// Common system managed metadata properties.
        /// </summary>
        [JsonProperty("sys")]
        public SystemProperties SystemProperties { get; set; }
        /// <summary>
        /// The next URL to call to fetch delta updates between this sync and the next one.
        /// </summary>
        public string NextSyncUrl { get; set; }
        /// <summary>
        /// Indicates that there are further results to be fetched in the current sync operation.
        /// </summary>
        public string NextPageUrl { get; set; }

        /// <summary>
        /// The <see cref="IEnumerable{T}"/> of <seealso cref="Entry{T}"/> fetched by the sync operation. 
        /// Note that the entry fields are returned as a dynamic as the data structure potentially contains elements
        /// that are not serializable directly to a class. For more information refer to <a href="https://www.contentful.com/developers/docs/references/content-delivery-api/#/reference/synchronization">the documentation</a>.
        /// </summary>
        public IEnumerable<Entry<dynamic>> Entries { get; set; }

        /// <summary>
        /// The <see cref="IEnumerable{T}"/> of <seealso cref="SyncedAsset"/> fetched by the sync operation.
        /// Note that the asset fields are returned as a dynamic as the data structure potentially contains elements
        /// that are not serializable directly to a class. For more information refer to <a href="https://www.contentful.com/developers/docs/references/content-delivery-api/#/reference/synchronization">the documentation</a>.
        /// </summary>
        public IEnumerable<SyncedAsset> Assets { get; set; }

        /// <summary>
        /// The <see cref="IEnumerable{T}"/> of deleted assets fetched by the sync operation.
        /// </summary>
        public IEnumerable<SystemProperties> DeletedAssets { get; set; }

        /// <summary>
        /// The <see cref="IEnumerable{T}"/> of deleted entries fetched by the sync operation.
        /// </summary>
        public IEnumerable<SystemProperties> DeletedEntries { get; set; }
    }
}
