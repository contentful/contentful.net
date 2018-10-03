using Contentful.Core.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Contentful.AspNetCore.TagHelpers
{
    /// <summary>
    /// Taghelper that renders a structured authoring field.
    /// </summary>
    [HtmlTargetElement("contentful-structured-text", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class ContentfulStructuredTextTagHelper : TagHelper
    {
        private HtmlRenderer _htmlRenderer;

        /// <summary>
        /// The document to render.
        /// </summary>
        public Document Document { get; set; }

        /// <summary>
        /// Creates a new instance of ContentfulStructuredTextTagHelper.
        /// </summary>
        /// <param name="renderer">The HtmlRenderer used to render the document.</param>
        public ContentfulStructuredTextTagHelper(HtmlRenderer renderer)
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
