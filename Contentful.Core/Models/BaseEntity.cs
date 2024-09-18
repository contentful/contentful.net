using Newtonsoft.Json;

namespace Contentful.Core.Models;

public class BaseEntity
{
    /// <summary>
    /// Common system managed metadata properties.
    /// </summary>
    [JsonProperty("sys")]
    public SystemProperties SystemProperties { get; set; }
}