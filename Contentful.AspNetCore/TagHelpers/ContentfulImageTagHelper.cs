using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Contentful.Core;
using Contentful.Core.Images;
using System.Threading.Tasks;

namespace Contentful.AspNetCore.TagHelpers
{
    public class ContentfulImageTagHelper : TagHelper
    {
        private readonly IContentfulClient _client;

        public ContentfulImageTagHelper(IContentfulClient client)
        {
            _client = client;
        }

        public string AssetId { get; set; }
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public ImageFormat Format { get; set; }
        public ImageResizeBehaviour ResizeBehaviour { get; set; }
        public ImageFocusArea FocusArea { get; set; }
        public int? JpgQuality { get; set; }
        public bool ProgressiveJpg { get; set; }
        public int? CornerRadius { get; set; }
        public string BackgroundColor { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            if (string.IsNullOrEmpty(Url))
            {
                var asset = await _client.GetAssetAsync(AssetId);
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
