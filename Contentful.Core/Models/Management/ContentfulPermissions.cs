using Contentful.Core.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
    [JsonConverter(typeof(ContentfulPermissionsJsonConverter))]
    public class ContentfulPermissions
    {
        [JsonProperty("ContentModel")]
        public List<string> ContentModel { get; set; }
        [JsonProperty("Settings")]
        public List<string> Settings { get; set; }
        [JsonProperty("ContentDelivery")]
        public List<string> ContentDelivery { get; set; }
    }
}
