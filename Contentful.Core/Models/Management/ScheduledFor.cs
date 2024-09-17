using System;
using Newtonsoft.Json;

namespace Contentful.Core.Models.Management;

public class ScheduledFor
{
    [JsonProperty("datetime")]
    public DateTime DateTime { get; set; }
    public string Timezone { get; set; }
}