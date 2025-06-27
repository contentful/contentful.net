using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Contentful.Core.Models;

public class FieldStatus
{
    [JsonProperty(PropertyName = "*")]
    public Dictionary<string, FieldStatusType> Status { get; set; }
}

public enum FieldStatusType
{
    [JsonProperty(PropertyName = "changed")]
    Changed,
    [JsonProperty(PropertyName = "draft")]
    Draft,    
    [JsonProperty(PropertyName = "published")]
    Published,
    [JsonProperty(PropertyName = "deleted")]
    Deleted,
}
