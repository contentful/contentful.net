using System;

namespace Contentful.Core.Models
{
    /// <summary>
    /// Represents a location field in a <seealso cref="Space"/>. 
    /// </summary>
    public class Location
    {
        /// <summary>
        /// The latitude of the location.
        /// </summary>
        public double Lat { get; set; }

        /// <summary>
        /// The longitude of the location.
        /// </summary>
        public double Lon { get; set; }
    }
}
