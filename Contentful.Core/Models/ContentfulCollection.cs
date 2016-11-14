using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Contentful.Core.Models
{
    /// <summary>
    /// Represents a collection of contentful resources with additional medadata regarding, skip, limit and total amount of items.
    /// </summary>
    /// <typeparam name="T">The type to serialize the items array from the API response into. Must be of type <seealso cref="IContentfulResource"/>.</typeparam>
    [JsonObject]
    public class ContentfulCollection<T> : IEnumerable<T> where T : IContentfulResource
    {
        /// <summary>
        /// Common system managed metadata properties.
        /// </summary>
        [JsonProperty("sys")]
        public SystemProperties SystemProperties { get; set; }

        /// <summary>
        /// The number of items skipped in this resultset.
        /// </summary>
        public int Skip { get; set; }

        /// <summary>
        /// The maximum number of items returned in this result.
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// The total number of items available.
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// The <see cref="IEnumerable{T}"/> of items to be serialized from the API response.
        /// </summary>
        public IEnumerable<T> Items { get; set; }

        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
