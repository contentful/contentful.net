using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Represents a single locale in a <see cref="Space"/>.
    /// </summary>
    public class Locale : IContentfulResource
    {
        /// <summary>
        /// Common system managed metadata properties.
        /// </summary>
        [JsonProperty("sys")]
        public SystemProperties SystemProperties { get; set; }

        /// <summary>
        /// The name of the locale.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The culture code for the locale.
        /// </summary>
        public string Code{ get; set; }

        /// <summary>
        /// The code of the locale to use as a fallback for this one.
        /// </summary>
        public string FallbackCode { get; set; }

        /// <summary>
        /// Whether or not this locale is the default one for this <see cref="Space"/>
        /// </summary>
        public bool Default { get; set; }

        /// <summary>
        /// Whether or not this locale is optional.
        /// </summary>
        public bool Optional { get; set; }

        /// <summary>
        /// Whether or not this locale is available in the management api.
        /// </summary>
        public bool ContentManagementApi { get; set; }

        /// <summary>
        /// Whether or not this locale is available in the delivery api.
        /// </summary>
        public bool ContentDeliveryApi { get; set; }
    }
}
