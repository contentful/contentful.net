using Contentful.Core.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Represents a collection of permissions for different parts of the Contentful APIs.
    /// </summary>
    public class ContentfulPermissions
    {
        [JsonProperty("ContentModel")]
        [JsonConverter(typeof(StringAllToListJsonConverter))]
        public List<string> ContentModel { get; set; }
        [JsonProperty("Settings")]
        [JsonConverter(typeof(StringAllToListJsonConverter))]
        public List<string> Settings { get; set; }
        [JsonProperty("ContentDelivery")]
        [JsonConverter(typeof(StringAllToListJsonConverter))]
        public List<string> ContentDelivery { get; set; }
    }
}
