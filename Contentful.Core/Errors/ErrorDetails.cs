using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contentful.Core.Errors
{
    /// <summary>
    /// Represents detailed information about a <see cref="ContentfulException"/>
    /// </summary>
    public class ErrorDetails
    {
        /// <summary>
        /// The dynamic representation of errors returned from the API.
        /// </summary>
        public dynamic Errors { get; set; }
    }
}
