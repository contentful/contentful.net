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

        public string UnitOfMeasure { get; set; }
        public string Interval { get; set; }

        /// <summary>
        /// The start date of the period.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// The end date of the period.
        /// </summary>
        public DateTime? EndDate { get; set; }

        public List<long> Usage { get; set; }
    }
}
