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
        public List<string> ContentModel { get; set; }
        public List<string> Settings { get; set; }
        public List<string> ContentDelivery { get; set; }
    }
}
