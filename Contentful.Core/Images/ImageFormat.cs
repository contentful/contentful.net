using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Images
{
    /// <summary>
    /// An enumeration representing the different kind of image formats available for images in Contentful.
    /// </summary>
    public enum ImageFormat
    {
        /// <summary>
        /// Keeps the original image format.
        /// </summary>
        Default,
        /// <summary>
        /// Turns the image into a JPG.
        /// </summary>
        Jpg,
        /// <summary>
        /// Turns the image into a PNG.
        /// </summary>
        Png,
        /// <summary>
        /// Turns the image into a WEBP.
        /// </summary>
        Webp
    }
}
