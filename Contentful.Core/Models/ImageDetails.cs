using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contentful.Core.Models
{
    /// <summary>
    /// Represents details about an image <see cref="File"/>.
    /// </summary>
    public class ImageDetails
    {
        /// <summary>
        /// The original height of the image.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The original width of the image.
        /// </summary>
        public int Width { get; set; }
    }
}
