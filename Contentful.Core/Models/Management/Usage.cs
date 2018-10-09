using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Represents the usage of a specific period of the Contentful API.
    /// </summary>
    public class ApiUsage
    {
        /// <summary>
        /// Common system managed metadata properties.
        /// </summary>
        [JsonProperty("sys")]
        public SystemProperties SystemProperties { get; set; }

        /// <summary>
        /// The unit that this instance of usage is measuring.
        /// </summary>
        public string UnitOfMeasure { get; set; }

        /// <summary>
        /// The interval being measured.
        /// </summary>
        public string Interval { get; set; }

        /// <summary>
        /// The start date of the period.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// The end date of the period.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// The usage for the period.
        /// </summary>
        public List<long> Usage { get; set; }
    }
}
