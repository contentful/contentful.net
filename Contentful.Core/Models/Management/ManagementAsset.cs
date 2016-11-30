using Contentful.Core.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Represents an asset in Contentfuls management API.
    /// </summary>
    [JsonConverter(typeof(ManagementAssetJsonConverter))]
    public class ManagementAsset : IContentfulResource
    {
        /// <summary>
        /// Common system managed metadata properties.
        /// </summary>
        [JsonProperty("sys")]
        public SystemProperties SystemProperties { get; set; }

        /// <summary>
        /// The titles of the asset.
        /// </summary>
        public Dictionary<string, string> Title { get; set; }

        /// <summary>
        /// The descriptions of the asset.
        /// </summary>
        public Dictionary<string,string> Description { get; set; }

        /// <summary>
        /// Information about the file in respective language.
        /// </summary>
        [JsonProperty("file")]
        public Dictionary<string,File> Files { get; set; }
    }
}
