using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Contentful.Core.Models.Management;

namespace Contentful.Core.Models
{
    /// <summary>
    /// Represents a single space.
    /// </summary>
    public class Space : IContentfulResource
    {
        /// <summary>
        /// Common system managed metadata properties.
        /// </summary>
        [JsonProperty("sys")]
        public SystemProperties SystemProperties { get; set; }

        /// <summary>
        /// The name of the space.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A list of locales this space supports.
        /// </summary>
        public List<Locale> Locales { get; set; }
    }
}
