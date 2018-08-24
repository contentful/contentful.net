using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Contentful.Core;
using Contentful.Core.Images;
using System.Threading.Tasks;
using System.Linq;
using Contentful.Core.Models;

namespace Contentful.AspNetCore.TagHelpers
{
    /// <summary>
    /// Base class for image taghelpers.
    /// </summary>
    public abstract class ImageTagHelperBase : TagHelper {

        /// <summary>
        /// The IContentfulClient that fetches assets from Contentful.
        /// </summary>
        protected IContentfulClient _client;

        /// <summary>
        /// The asset to display information about. If set takes precedence over the AssetId and Url properties.
        /// </summary>
        public Asset Asset { get; set; }

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
        /// Builds a url to a contentful image using the specified properties.
        /// </summary>
        /// <returns>The url.</returns>
        public async Task<string> BuildUrl()
        {
            if(Asset != null)
            {
                Url = Asset.File?.Url;
                if (string.IsNullOrEmpty(Url))
                {
                    AssetId = Asset.SystemProperties?.Id;
                }
            }

            if (string.IsNullOrEmpty(Url))
            {
                Asset = await _client.GetAsset(AssetId, "");
                Url = Asset.File.Url;
            }

            var contentType = Asset?.File?.ContentType;

            if(contentType == null && !string.IsNullOrEmpty(Url))
            {
                contentType = Url.ToLower().EndsWith(".jpg") || Url.ToLower().EndsWith(".jpeg") ? "image/jpeg" : "";
            }

            var isJpg = (contentType?.ToLower() == "image/jpeg" && Format == ImageFormat.Default) || Format == ImageFormat.Jpg;

            var queryBuilder = new ImageUrlBuilder();

            if (Width > 0)
            {
                queryBuilder.SetWidth(Width);
            }

            if (Height > 0)
            {
                queryBuilder.SetHeight(Height);
            }

            if (JpgQuality.HasValue && isJpg)
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

            if (ProgressiveJpg && isJpg)
            {
                queryBuilder.UseProgressiveJpg();
            }

            return $"{Url}{queryBuilder.Build()}";
        }
    }

    /// <summary>
    /// TagHelper to create an img tag for a Contentful asset.
    /// </summary>
    [RestrictChildren("contentful-source")]
    public class ContentfulImageTagHelper : ImageTagHelperBase
    {
        /// <summary>
        /// Creates a new instance of ContentfulImageTagHelper.
        /// </summary>
        /// <param name="client">The IContentfulClient used to retrieve the asset.</param>
        public ContentfulImageTagHelper(IContentfulClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Executes the taghelper.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var sources = new List<string>();
            context.Items.Add("sources", sources);
            context.Items.Add("defaults", this);

            await output.GetChildContentAsync();

            output.TagName = "img";
            output.Attributes.Add("src", await BuildUrl());

            if (!context.AllAttributes.ContainsName("alt"))
            {
                output.Attributes.Add("alt", Asset?.Description);
            }

            if (sources.Any())
            {
                output.Attributes.Add("srcset", string.Join(", ", sources));
            }
        }
    }

    /// <summary>
    /// Taghelper that represents a single source in a source set.
    /// </summary>
    [HtmlTargetElement(ParentTag = "contentful-image")]
    public class ContentfulSource : ImageTagHelperBase
    {
        /// <summary>
        /// The size specification for this source.
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// Creates a new instance of ContentfulSource.
        /// </summary>
        /// <param name="client">The IContentfulClient used to retrieve the asset.</param>
        public ContentfulSource(IContentfulClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Executes the taghelper.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var defaults = context.Items["defaults"] as ContentfulImageTagHelper;
            SetDefaults(defaults);
            var sources = context.Items["sources"] as List<string>;
            var url = await BuildUrl();

            if (string.IsNullOrEmpty(Size) && Width > 0)
            {
                Size = $"{Width}w";
            }

            sources.Add($"{url} {Size}");

            output.SuppressOutput();
        }

        private void SetDefaults(ContentfulImageTagHelper defaults)
        {
            if(defaults == null)
            {
                return;
            }

            ProgressiveJpg = defaults.ProgressiveJpg || ProgressiveJpg;
            ResizeBehaviour = ResizeBehaviour != ImageResizeBehaviour.Default ? ResizeBehaviour : defaults.ResizeBehaviour;
            FocusArea = FocusArea != ImageFocusArea.Default ? FocusArea : defaults.FocusArea;
            Format = Format != ImageFormat.Default ? Format : defaults.Format;
            Width = Width > 0 ? Width : defaults.Width;
            Height = Height > 0 ? Height : defaults.Height;
            JpgQuality = JpgQuality.HasValue ? JpgQuality : defaults.JpgQuality;
            CornerRadius = CornerRadius.HasValue ? CornerRadius : defaults.CornerRadius;
            BackgroundColor = string.IsNullOrEmpty(BackgroundColor) ? defaults.BackgroundColor : BackgroundColor;
            Url = string.IsNullOrEmpty(Url) ? defaults.Url : Url;
            AssetId = string.IsNullOrEmpty(AssetId) ? defaults.AssetId : AssetId;
            Asset = Asset ?? defaults.Asset;
        }
    }
}
