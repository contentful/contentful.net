using Newtonsoft.Json;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Represents a JSON Patch operation.
    /// </summary>
    public class JsonPatchOperation
    {
        /// <summary>
        /// The operation to perform (add, remove, replace, etc.).
        /// </summary>
        [JsonProperty("op")]
        public string Operation { get; set; }

        /// <summary>
        /// The path to the property to modify.
        /// </summary>
        [JsonProperty("path")]
        public string Path { get; set; }

        /// <summary>
        /// The value to set (for add/replace operations).
        /// </summary>
        [JsonProperty("value")]
        public object Value { get; set; }
    }
} 