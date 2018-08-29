using Contentful.Core.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Contentful.AspNetCore.TagHelpers
{
    [HtmlTargetElement("contentful-structured-text", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class ContentfulStructuredTextTagHelper : TagHelper
    {
        private HtmlRenderer _htmlRenderer;

        public Document Document { get; set; }

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
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if(Document == null)
            {
                return;
            }

            var html = _htmlRenderer.ToHtml(Document);

            output.Content.SetHtmlContent(html);
        }
    }
}
