using Contentful.Core.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
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
