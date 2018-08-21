using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contentful.Core.Images
{
    /// <summary>
    /// Utility builder class to construct a correct image manipulation query string for a Contentful image.
    /// </summary>
    public class ImageUrlBuilder
    {
        private readonly List<KeyValuePair<string, string>> _querystringValues = new List<KeyValuePair<string, string>>();

        /// <summary>
        /// Creates a new instance of an ImageUrlBuilder.
        /// </summary>
        /// <returns>The created <see cref="ImageUrlBuilder"/>.</returns>
        public static ImageUrlBuilder New()
        {
            return new ImageUrlBuilder();
        }

        /// <summary>
        /// Sets the format of the image returned.
        /// </summary>
        /// <param name="format">The <see cref="ImageFormat"/> of the image returned.</param>
        /// <returns>The <see cref="ImageUrlBuilder"/> instance.</returns>
        public ImageUrlBuilder SetFormat(ImageFormat format)
        {
            if(format == ImageFormat.Default)
            {
                return this;
            }

            if(_querystringValues.Any(c => c.Key == "fm"))
            {
                return this;
            }

            _querystringValues.Add(new KeyValuePair<string, string>("fm", format.ToString().ToLower()));
            return this;
        }

        /// <summary>
        /// Sets the quality of the jpg image returned.
        /// </summary>
        /// <param name="quality">The quality as a percentage between 0 and 100.</param>
        /// <returns>The <see cref="ImageUrlBuilder"/> instance.</returns>
        public ImageUrlBuilder SetJpgQuality(int quality)
        {
            _querystringValues.Add(new KeyValuePair<string, string>("q", quality.ToString()));
            return this;
        }

        /// <summary>
        /// Sets the color depth of the png image returned to 8 bit.
        /// </summary>
        /// <returns>The <see cref="ImageUrlBuilder"/> instance.</returns>
        public ImageUrlBuilder Set8BitPng()
        {
            if(!_querystringValues.Any(c => c.Key == "fm"))
            {
                SetFormat(ImageFormat.Png);
            }
            else if (!_querystringValues.Any(c => c.Key == "fm" && c.Value == "png"))
            {
                throw new ArgumentException("The format must be set to png when using the 8 bit png color depth.");
            }
            _querystringValues.Add(new KeyValuePair<string, string>("fl", "png8"));
            return this;
        }

        /// <summary>
        /// Sets the returned jpg to be progressive.
        /// </summary>
        /// <returns>The <see cref="ImageUrlBuilder"/> instance.</returns>
        public ImageUrlBuilder UseProgressiveJpg()
        {
            _querystringValues.Add(new KeyValuePair<string, string>("fl", "progressive"));
            return this;
        }

        /// <summary>
        /// Sets the width of the returned image.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <returns>The <see cref="ImageUrlBuilder"/> instance.</returns>
        public ImageUrlBuilder SetWidth(int width)
        {
            _querystringValues.Add(new KeyValuePair<string, string>("w", width.ToString()));
            return this;
        }

        /// <summary>
        /// Sets the height of the returned image.
        /// </summary>
        /// <param name="height">The height of the image.</param>
        /// <returns>The <see cref="ImageUrlBuilder"/> instance.</returns>
        public ImageUrlBuilder SetHeight(int height)
        {
            _querystringValues.Add(new KeyValuePair<string, string>("h", height.ToString()));
            return this;
        }

        /// <summary>
        /// Sets how an image should be resized to fit inside the height and width specified.
        /// </summary>
        /// <param name="resizeBehaviour">The resizebehaviour.</param>
        /// <returns>The <see cref="ImageUrlBuilder"/> instance.</returns>
        public ImageUrlBuilder SetResizingBehaviour(ImageResizeBehaviour resizeBehaviour)
        {
            if(resizeBehaviour == ImageResizeBehaviour.Default)
            {
                return this;
            }

            _querystringValues.Add(new KeyValuePair<string, string>("fit", resizeBehaviour.ToString().ToLower()));
            return this;
        } 

        /// <summary>
        /// Sets what focus area should be used when resizing.
        /// </summary>
        /// <param name="focusArea">The area to focus the image on.</param>
        /// <returns>The <see cref="ImageUrlBuilder"/> instance.</returns>
        public ImageUrlBuilder SetFocusArea(ImageFocusArea focusArea)
        {
            if(focusArea == ImageFocusArea.Default)
            {
                return this;
            }

            _querystringValues.Add(new KeyValuePair<string, string>("f", focusArea.ToString().ToLower()));
            return this;
        }

        /// <summary>
        /// Set the corner radius of the image.
        /// </summary>
        /// <param name="radius">The radius to set.</param>
        /// <returns>The <see cref="ImageUrlBuilder"/> instance.</returns>
        public ImageUrlBuilder SetCornerRadius(int radius)
        {
            _querystringValues.Add(new KeyValuePair<string, string>("r", radius.ToString()));
            return this;
        }

        /// <summary>
        /// Sets the background color when using <see cref="ImageResizeBehaviour.Pad"/> or <see cref="SetCornerRadius"/>.
        /// </summary>
        /// <param name="rgbColor">The background color in RGB format, starts with "rgb:" or "#" e.g. rgb:000080 or #000080.</param>
        /// <returns></returns>
        public ImageUrlBuilder SetBackgroundColor(string rgbColor)
        {
            if (string.IsNullOrEmpty(rgbColor))
            {
                return this;
            }

            if (rgbColor.StartsWith("#"))
            {
                rgbColor = "rgb:" + rgbColor.Substring(1);
            }

            _querystringValues.Add(new KeyValuePair<string, string>("bg", rgbColor));

            return this;
        }

        /// <summary>
        /// Builds the query and returns the formatted querystring.
        /// </summary>
        /// <returns>The formatted querystring.</returns>
        public string Build()
        {
            ClearImproperValues();
            var sb = new StringBuilder();
            var hasQuery = false;

            foreach (var parameter in _querystringValues)
            {
                sb.Append(hasQuery ? '&' : '?');
                sb.Append(parameter.Key);
                sb.Append('=');
                sb.Append(parameter.Value);
                hasQuery = true;
            }

            return sb.ToString();
        }

        private void ClearImproperValues()
        {
            // If the querystrings contain both jpg specific values and a specific format, make sure the format is jpg, else remove the jpg specific values.
            if (_querystringValues.Any(c => c.Key == "q" || c.Key == "fl") && _querystringValues.Any(c => c.Key == "fm")){
                var format = _querystringValues.First(c => c.Key == "fm").Value;

                if(format != "jpg")
                {
                    var quality = _querystringValues.FirstOrDefault(c => c.Key == "q");
                    var progressive = _querystringValues.FirstOrDefault(c => c.Key == "fl" && c.Value == "progressive");

                    _querystringValues.Remove(quality);
                    _querystringValues.Remove(progressive);
                }
            }
        }
    }
}
