using Contentful.Core;
using Contentful.Core.Models;
using Contentful.Core.Search;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Contentful.AspNetCore.TagHelpers
{
    /// <summary>
    /// Taghelper to create an anchor link for a Contentful asset.
    /// </summary>
    [HtmlTargetElement("a", Attributes = "asset-id")]
    public class ContentfulAssetTagHelper : TagHelper
    {
        private readonly IContentfulClient _client;

        /// <summary>
        /// Creates a new instance of a ContentfulAssetTagHelper.
        /// </summary>
        /// <param name="client">The IContentfulClient used to retrieve the asset.</param>
        public ContentfulAssetTagHelper(IContentfulClient client)
        {
            _client = client;
        }

        /// <summary>
        /// The id of the asset.
        /// </summary>
        public string AssetId { get; set; }

        /// <summary>
        /// The locale of the asset.
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// Executes the taghelper.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (string.IsNullOrEmpty(AssetId))
            {
                return;
            }

            var queryBuilder = QueryBuilder<Asset>.New;

            if (!string.IsNullOrEmpty(Locale))
            {
                queryBuilder = queryBuilder.LocaleIs(Locale);
            }

            var asset = await _client.GetAsset(AssetId, queryBuilder);

            output.Attributes.RemoveAll("asset-id");
            output.Attributes.RemoveAll("locale");

            output.Attributes.SetAttribute("href", asset.File.Url);
        }
    }
}
