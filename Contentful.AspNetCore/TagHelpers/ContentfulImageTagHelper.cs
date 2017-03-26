using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Contentful.Core;
using Contentful.Core.Images;
using System.Threading.Tasks;

namespace Contentful.AspNetCore.TagHelpers
{
    /// <summary>
    /// TagHelper to create an img tag for a Contentful asset.
    /// </summary>
    public class ContentfulImageTagHelper : TagHelper
    {
        private readonly IContentfulClient _client;

        /// <summary>
        /// Creates a new instance of ContentfulImageTagHelper.
        /// </summary>
        /// <param name="client">The IContentfulClient used to retrieve the asset.</param>
        public ContentfulImageTagHelper(IContentfulClient client)
        {
            _client = client;
        }

        /// <summary>
        /// The id of the asset.
        /// </summary>
        public string AssetId { get; set; }

        /// <summary>
        /// The url of the asset.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The width of the image.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The height of the image.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The format of the image.
        /// </summary>
        public ImageFormat Format { get; set; }

        /// <summary>
        /// The resize behaviour of the image.
        /// </summary>
        public ImageResizeBehaviour ResizeBehaviour { get; set; }

        /// <summary>
        /// The area to focus on in the image.
        /// </summary>
        public ImageFocusArea FocusArea { get; set; }

        /// <summary>
        /// The quality of the image as a value between 0 and 100.
        /// </summary>
        public int? JpgQuality { get; set; }

        /// <summary>
        /// Wether progressive JPGs should be used.
        /// </summary>
        public bool ProgressiveJpg { get; set; }

        /// <summary>
        /// The corner radius of the image.
        /// </summary>
        public int? CornerRadius { get; set; }

        /// <summary>
        /// The backgroundcolor of the image when padded.
        /// </summary>
        public string BackgroundColor { get; set; }

        /// <summary>
        /// Executes the taghelper.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            if (string.IsNullOrEmpty(Url))
            {
                var asset = await _client.GetAssetAsync(AssetId, "");
                Url = asset.File.Url;
            }

            var queryBuilder = new ImageUrlBuilder();

            if(Width > 0)
            {
                queryBuilder.SetWidth(Width);
            }

            if(Height > 0)
            {
                queryBuilder.SetHeight(Height);
            }

            if (JpgQuality.HasValue)
            {
                queryBuilder.SetJpgQuality(JpgQuality.Value);
            }

            if (CornerRadius.HasValue)
            {
                queryBuilder.SetCornerRadius(CornerRadius.Value);
            }

            if (!string.IsNullOrEmpty(BackgroundColor))
            {
                queryBuilder.SetBackgroundColor(BackgroundColor);
            }

            queryBuilder.SetFocusArea(FocusArea);
            queryBuilder.SetResizingBehaviour(ResizeBehaviour);
            queryBuilder.SetFormat(Format);

            if (ProgressiveJpg)
            {
                queryBuilder.UseProgressiveJpg();
            }

            output.TagName = "img";
            output.Attributes.Add("src", Url + queryBuilder.Build());
        }
    }
}
