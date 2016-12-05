using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Images
{
    /// <summary>
    /// Represents the different kinds of resizing behaviours for images available.
    /// </summary>
    public enum ImageResizeBehaviour
    {
        /// <summary>
        /// Resizes the image to fit within the bounding box specified by the width and height parameters, while maintaining aspect ratio.
        /// </summary>
        Default,
        /// <summary>
        /// Pads the image so that the resulting image has the exact size of the width and height parameters.
        /// </summary>
        Pad,
        /// <summary>
        /// Crops the image to match the specified size.
        /// </summary>
        Crop,
        /// <summary>
        /// Crops the image to match the specified size, if the original image is smaller it will be upscaled.
        /// </summary>
        Fill,
        /// <summary>
        /// Creates a thumbnail from an image based on a focus area.
        /// </summary>
        Thumb,
        /// <summary>
        /// Scale the image regardless of original aspect ratio.
        /// </summary>
        Scale
    }
}
