using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contentful.Core.Models
{
    /// <summary>
    /// Represents a single Contentful resource.
    /// </summary>
    public interface IContentfulResource
    {
        /// <summary>
        /// Common system managed metadata properties.
        /// </summary>
        SystemProperties SystemProperties { get; set; }
    }
}
