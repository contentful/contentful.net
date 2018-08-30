using Contentful.Core.Models.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contentful.Core.Models
{
    /// <summary>
    /// Renderer that turns a document into HTML.
    /// </summary>
    public class HtmlRenderer
    {
        private readonly ContentRenderererCollection _contentRenderererCollection;

        /// <summary>
        /// Initializes a new instance of HtmlRenderer.
        /// </summary>
        public HtmlRenderer()
        {
            _contentRenderererCollection = new ContentRenderererCollection();
            _contentRenderererCollection.AddRenderers(new List<IContentRenderer> {
                new ParagraphRenderer(_contentRenderererCollection),
                new HyperlinkContentRenderer(_contentRenderererCollection),
                new TextRenderer(),
                new HeadingRenderer(_contentRenderererCollection),
                new AssetRenderer(),
                new NullContentRenderer()
            });
        }

        /// <summary>
        /// Renders a document to HTML.
        /// </summary>
        /// <param name="doc">The document to turn into HTML.</param>
        /// <returns>An HTML string.</returns>
        public string ToHtml(Document doc)
        {
            var sb = new StringBuilder();
            foreach (var content in doc.Content)
            {
                var renderer = _contentRenderererCollection.GetRendererForContent(content);
                sb.Append(renderer.Render(content));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Adds a contentrenderer to the rendering pipeline.
        /// </summary>
        /// <param name="renderer"></param>
        public void AddRenderer(IContentRenderer renderer)
        {
            _contentRenderererCollection.AddRenderer(renderer);
        }
    }

    /// <summary>
    /// A collection of renderers.
    /// </summary>
    public class ContentRenderererCollection
    {
        readonly List<IContentRenderer> _renderers = new List<IContentRenderer>();

        /// <summary>
        /// Adds a renderer to the collection.
        /// </summary>
        /// <param name="renderer"></param>
        public void AddRenderer(IContentRenderer renderer)
        {
            _renderers.Add(renderer);
        }

        /// <summary>
        /// Adds multiple renderers to the collection.
        /// </summary>
        /// <param name="collection"></param>
        public void AddRenderers(IEnumerable<IContentRenderer> collection)
        {
            _renderers.AddRange(collection);
        }

        /// <summary>
        /// Gets the first available renderer for a specific IContent.
        /// </summary>
        /// <param name="content">The content to get a renderer for.</param>
        /// <returns>The found renderer or null.</returns>
        public IContentRenderer GetRendererForContent(IContent content)
        {
            return _renderers.OrderBy(c => c.Order).FirstOrDefault(c => c.SupportsContent(content));
        }
    }

    /// <summary>
    /// Interface representing a content renderer.
    /// </summary>
    public interface IContentRenderer
    {
        /// <summary>
        /// The order this renderer should have in the collection of renderers.
        /// </summary>
        int Order { get; set; }

        /// <summary>
        /// Whether or not the renderer supports the specified content.
        /// </summary>
        /// <param name="content">The content to evaluate for rendering.</param>
        /// <returns>True if the renderer can render the provided content, otherwise false.</returns>
        bool SupportsContent(IContent content);

        /// <summary>
        /// Renders the provided content as a string.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns></returns>
        string Render(IContent content);
    }

    /// <summary>
    /// A renderer for a paragraph.
    /// </summary>
    public class ParagraphRenderer : IContentRenderer
    {
        private readonly ContentRenderererCollection _renderererCollection;

        /// <summary>
        /// Initializes a new PragraphRenderer
        /// </summary>
        /// <param name="renderererCollection">The collection of renderer to use for sub-content.</param>
        public ParagraphRenderer(ContentRenderererCollection renderererCollection)
        {
            _renderererCollection = renderererCollection;
        }

        /// <summary>
        /// The order of this renderer in the collection.
        /// </summary>
        public int Order { get; set; } = 100;

        /// <summary>
        /// Whether or not this renderer supports the provided content.
        /// </summary>
        /// <param name="content">The content to evaluate.</param>
        /// <returns>Returns true if the content is a paragraph, otherwise false.</returns>
        public bool SupportsContent(IContent content)
        {
            return content is Paragraph;
        }

        /// <summary>
        /// Renders the content to an html p-tag.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns>The p-tag as a string.</returns>
        public string Render(IContent content)
        {
            var paragraph = content as Paragraph;
            var sb = new StringBuilder();
            sb.Append("<p>");

            foreach (var subContent in paragraph.Content)
            {
                var renderer = _renderererCollection.GetRendererForContent(subContent);
                sb.Append(renderer.Render(subContent));
            }

            sb.Append("</p>");
            return sb.ToString();
        }
    }

    /// <summary>
    /// A renderer for a heading.
    /// </summary>
    public class HeadingRenderer : IContentRenderer
    {
        private readonly ContentRenderererCollection _renderererCollection;

        /// <summary>
        /// Initializes a new HeadingRenderer.
        /// </summary>
        /// <param name="renderererCollection">The collection of renderer to use for sub-content.</param>
        public HeadingRenderer(ContentRenderererCollection renderererCollection)
        {
            _renderererCollection = renderererCollection;
        }

        /// <summary>
        /// The order of this renderer in the collection.
        /// </summary>
        public int Order { get; set; } = 100;

        /// <summary>
        /// Whether or not this renderer supports the provided content.
        /// </summary>
        /// <param name="content">The content to evaluate.</param>
        /// <returns>Returns true if the content is a heading, otherwise false.</returns>
        public bool SupportsContent(IContent content)
        {
            return content is Heading;
        }

        /// <summary>
        /// Renders the content to an html h-tag.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns>The p-tag as a string.</returns>
        public string Render(IContent content)
        {
            var heading = content as Heading;
            var sb = new StringBuilder();
            sb.Append($"<h{heading.HeadingSize}>");

            foreach (var subContent in heading.Content)
            {
                var renderer = _renderererCollection.GetRendererForContent(subContent);
                sb.Append(renderer.Render(subContent));
            }

            sb.Append($"</h{heading.HeadingSize}>");
            return sb.ToString();
        }
    }

    /// <summary>
    /// A renderer for a text node.
    /// </summary>
    public class TextRenderer : IContentRenderer
    {
        /// <summary>
        /// The order of this renderer in the collection.
        /// </summary>
        public int Order { get; set; } = 100;

        /// <summary>
        /// Whether or not this renderer supports the provided content.
        /// </summary>
        /// <param name="content">The content to evaluate.</param>
        /// <returns>Returns true if the content is a textual node, otherwise false.</returns>
        public bool SupportsContent(IContent content)
        {
            return content is Text;
        }

        public string Render(IContent content)
        {
            var text = content as Text;
            var sb = new StringBuilder();

            if (text.Marks != null)
            {
                foreach (var mark in text.Marks)
                {
                    sb.Append($"<{MarkToHtmlTag(mark)}>");
                }
            }

            sb.Append(text.Value);

            if (text.Marks != null)
            {
                foreach (var mark in text.Marks)
                {
                    sb.Append($"</{MarkToHtmlTag(mark)}>");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Renders the content to a string.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns>The textual string.</returns>
        private string MarkToHtmlTag(Mark mark)
        {
            switch (mark.Type)
            {
                case "bold":
                    return "strong";
                case "underline":
                    return "u";
                case "italic":
                    return "em";
            }

            return "span";
        }
    }

    /// <summary>
    /// A renderer for an asset.
    /// </summary>
    public class AssetRenderer : IContentRenderer
    {
        /// <summary>
        /// The order of this renderer in the collection.
        /// </summary>
        public int Order { get; set; } = 100;

        /// <summary>
        /// Whether or not this renderer supports the provided content.
        /// </summary>
        /// <param name="content">The content to evaluate.</param>
        /// <returns>Returns true if the content is an asset, otherwise false.</returns>
        public bool SupportsContent(IContent content)
        {
            return content is Asset;
        }

        /// <summary>
        /// Renders the content to a string.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns>The html img or a tag.</returns>
        public string Render(IContent content)
        {
            var asset = content as Asset;
            var sb = new StringBuilder();
            if(asset.File?.ContentType != null && asset.File.ContentType.ToLower().Contains("image"))
            {
                sb.Append($"<img src=\"{asset.File.Url}\" alt=\"{asset.Title}\" />");
            }else
            {
                sb.Append($"<a href=\"{asset.File.Url}\">{asset.Title}</a>");
            }

            return sb.ToString();
        }
    }

    /// <summary>
    /// A renderer for a hyperlink.
    /// </summary>
    public class HyperlinkContentRenderer : IContentRenderer
    {
        private readonly ContentRenderererCollection _renderererCollection;

        /// <summary>
        /// Initializes a new HyperlinkContentRenderer.
        /// </summary>
        /// <param name="renderererCollection">The collection of renderer to use for sub-content.</param>
        public HyperlinkContentRenderer(ContentRenderererCollection contentRenderererCollection)
        {
            _renderererCollection = contentRenderererCollection;
        }

        /// <summary>
        /// The order of this renderer in the collection.
        /// </summary>
        public int Order { get; set; } = 100;

        /// <summary>
        /// Whether or not this renderer supports the provided content.
        /// </summary>
        /// <param name="content">The content to evaluate.</param>
        /// <returns>Returns true if the content is a hyperlink, otherwise false.</returns>
        public bool SupportsContent(IContent content)
        {
            return content is Hyperlink;
        }

        /// <summary>
        /// Renders the content to a string.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns>The a tag as a string.</returns>
        public string Render(IContent content)
        {
            var link = content as Hyperlink;
            var sb = new StringBuilder();

            sb.Append($"<a href=\"{link.Data.Url}\" title=\"{link.Data.Title}\">");

            foreach (var subContent in link.Content)
            {
                var renderer = _renderererCollection.GetRendererForContent(subContent);
                sb.Append(renderer.Render(subContent));
            }

            sb.Append("</a>");

            return sb.ToString();
        }
    }

    /// <summary>
    /// A content renderer that doesn't output anything.
    /// </summary>
    public class NullContentRenderer : IContentRenderer
    {
        /// <summary>
        /// The order of this renderer in the collection.
        /// </summary>
        public int Order { get; set; } = 100;

        /// <summary>
        /// Renders the content to a string.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns>An empty string.</returns>
        public string Render(IContent content)
        {
            return "";
        }

        /// <summary>
        /// Whether or not this renderer supports the provided content.
        /// </summary>
        /// <param name="content">The content to evaluate.</param>
        /// <returns>Returns true for all content.</returns>
        public bool SupportsContent(IContent content)
        {
            return true;
        }
    }
}
