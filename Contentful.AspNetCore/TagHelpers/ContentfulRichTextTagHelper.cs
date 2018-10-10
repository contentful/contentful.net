using Contentful.Core.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Contentful.AspNetCore.TagHelpers
{
    /// <summary>
    /// Taghelper that renders a rich text field.
    /// </summary>
    [HtmlTargetElement("contentful-rich-text", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class ContentfulRichTextTagHelper : TagHelper
    {
        private HtmlRenderer _htmlRenderer;

        /// <summary>
        /// The document to render.
        /// </summary>
        public Document Document { get; set; }

        /// <summary>
        /// Creates a new instance of ContentfulRichTextTagHelper.
        /// </summary>
        /// <param name="renderer">The HtmlRenderer used to render the document.</param>
        public ContentfulRichTextTagHelper(HtmlRenderer renderer)
        {
            _htmlRenderer = renderer;
        }

        /// <summary>
        /// Executes the taghelper.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if(Document == null)
            {
                return;
            }

            var html = await _htmlRenderer.ToHtml(Document);

            output.Content.SetHtmlContent(html);
        }
    }
}
