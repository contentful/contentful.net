using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Contentful.Core.Models
{
    /// <summary>
    /// Represents a single entry of a <seealso cref="ContentType"/> in a <seealso cref="Space"/>. 
    /// </summary>
    /// <typeparam name="T">The type the fields of the entry should be serialized into.</typeparam>
    public class Entry<T> : IEntry<T>
    {
        /// <summary>
        /// Common system managed metadata properties.
        /// </summary>
        [JsonProperty("sys")]
        public SystemProperties SystemProperties { get; set; }

        /// <summary>
        /// The fields of the entry deserialized to the type T.
        /// </summary>
        public T Fields { get; set; }
    }
}
