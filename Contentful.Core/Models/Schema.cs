using Contentful.Core.Models.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contentful.Core.Models
{
    /// <summary>
    /// Represents a schema of array items.
    /// </summary>
    public class Schema
    {
        /// <summary>
        /// Specifies what types of resources are allowed in the array.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Specifies what type of links are allowed in the array.
        /// </summary>
        public string LinkType { get; set; }

        /// <summary>
        /// The validations that should be applied to the items in the array.
        /// </summary>
        public List<IFieldValidator> Validations { get; set; }
    }
}
