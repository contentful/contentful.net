using Contentful.Core;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Contentful.AspNetCore.TagHelpers
{
    [HtmlTargetElement("a", Attributes = "asset-id")]
    public class ContentfulAssetTagHelper : TagHelper
    {
        private readonly IContentfulClient _client;

        public ContentfulAssetTagHelper(IContentfulClient client)
        {
            _client = client;
        }

        public string AssetId { get; set; }

        public string Locale { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrEmpty(AssetId))
            {
                return;
            }

            var asset = await _client.GetAssetAsync(AssetId, Locale);

            output.Attributes.RemoveAll("asset-id");
            output.Attributes.RemoveAll("locale");

            output.Attributes.SetAttribute("href", asset.File.Url);
        }
    }
}
