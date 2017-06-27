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
    /// <summary>
    /// Taghelper that allows rendering img tags with source sets.
    /// </summary>
    [RestrictChildren("contentful-source")]
    public class ContentfulSourceSetsTagHelper : TagHelper
    {
        /// <summary>
        /// Executes the taghelper.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output) {
            var sourceContext = new ImageSourcesContext();
            context.Items.Add("sources", sourceContext);
            await output.GetChildContentAsync();

            output.TagName = "img";

            output.Attributes.Add("src", sourceContext.DefaultUrl);

            output.Attributes.Add("srcset", string.Join(", ", sourceContext.Sources));
        }
    }

    /// <summary>
    /// Taghelper that represents a single source in a source set.
    /// </summary>
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
            var sourcesContext = context.Items["sources"] as ImageSourcesContext;
            var url = await BuildUrl();
            sourcesContext.Sources.Add($"{url} {Size}");

            if (IsDefault)
            {
                sourcesContext.DefaultUrl = url;
            }

            output.SuppressOutput();
        }
    }

    internal class ImageSourcesContext
    {
        public ImageSourcesContext()
        {
            Sources = new List<string>();
        }

        public List<string> Sources { get; set; }
        public string DefaultUrl { get; set; }
    }
}
