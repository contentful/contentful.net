using Contentful.Core;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Contentful.AspNetCore.TagHelpers
{
    [RestrictChildren("source")]
    public class ContentfulSourceSetsTagHelper : TagHelper
    {
        /// <summary>
        /// Executes the taghelper.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output) {
            var sourceSets = new List<string>();
            context.Items["sources"] = sourceSets;

            await output.GetChildContentAsync();

            output.TagName = "img";

            output.Attributes.Add("srcset", string.Join(", ", sourceSets));
            output.Attributes.Add("src", context.Items["DefaultUrl"]);
        }
    }

    [HtmlTargetElement(ParentTag = "contentful-source-sets")]
    public class ContentfulSource : ImageTagHelperBase
    {
        /// <summary>
        /// The size specification for this source.
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// Whether or not this source should be used for the default src attribute.
        /// </summary>
        public bool IsDefault { get; set; }

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
            var list = context.Items["sources"] as List<string>;
            var query = await BuildQuery();
            list.Add($"{await BuildQuery()} {Size}");

            if (IsDefault)
            {
                context.Items["DefaultUrl"] = await BuildQuery();
            }

            output.SuppressOutput();
        }
    }
}
