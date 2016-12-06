using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Images
{
    /// <summary>
    /// Represents the different kinds of areas to focus an image on when using the <see cref="ImageResizeBehaviour.Thumb"/>.
    /// </summary>
    public enum ImageFocusArea
    {
        /// <summary>
        /// Center of image.
        /// </summary>
        Default,
        /// <summary>
        /// Top of the image.
        /// </summary>
        Top,
        /// <summary>
        /// Right side of the image.
        /// </summary>
        Right,
        /// <summary>
        /// Left side of the image.
        /// </summary>
        Left,
        /// <summary>
        /// Bottom side of the image.
        /// </summary>
        Bottom,
        /// <summary>
        /// Top right of the image.
        /// </summary>
        Top_Right,
        /// <summary>
        /// Top left of the image.
        /// </summary>
        Top_Left,
        /// <summary>
        /// Bottom right of the image.
        /// </summary>
        Bottom_Right,
        /// <summary>
        /// Bottom left of the image.
        /// </summary>
        Bottom_Left,
        /// <summary>
        /// Focuses on a face of an image using face detection.
        /// </summary>
        Face,
        /// <summary>
        /// Focuses on multiple faces of an image using face detection.
        /// </summary>
        Faces
    }
}
