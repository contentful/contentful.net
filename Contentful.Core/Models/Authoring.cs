using Contentful.Core.Models.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contentful.Core.Models
{
    /// <summary>
    /// Renderer that turns a document into HTML.
    /// </summary>
    public class HtmlRenderer
    {
        private readonly ContentRendererCollection _contentRendererCollection;

        /// <summary>
        /// Initializes a new instance of HtmlRenderer.
        /// </summary>
        public HtmlRenderer()
        {
            _contentRendererCollection = new ContentRendererCollection();
            _contentRendererCollection.AddRenderers(new List<IContentRenderer> {
                new ParagraphRenderer(_contentRendererCollection),
                new HyperlinkContentRenderer(_contentRendererCollection),
                new TextRenderer(),
                new HorizontalRulerContentRenderer(),
                new HeadingRenderer(_contentRendererCollection),
                new ListContentRenderer(_contentRendererCollection),
                new ListItemContentRenderer(_contentRendererCollection),
                new QuoteContentRenderer(_contentRendererCollection),
                new AssetRenderer(_contentRendererCollection),
                new NullContentRenderer()
            });
        }

        /// <summary>
        /// Renders a document to HTML.
        /// </summary>
        /// <param name="doc">The document to turn into HTML.</param>
        /// <returns>An HTML string.</returns>
        public async Task<string> ToHtml(Document doc)
        {
            var sb = new StringBuilder();
            foreach (var content in doc.Content)
            {
                var renderer = _contentRendererCollection.GetRendererForContent(content);
                sb.Append(await renderer.RenderAsync(content));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Adds a contentrenderer to the rendering pipeline.
        /// </summary>
        /// <param name="renderer"></param>
        public void AddRenderer(IContentRenderer renderer)
        {
            _contentRendererCollection.AddRenderer(renderer);
        }
    }

    /// <summary>
    /// A collection of renderers.
    /// </summary>
    public class ContentRendererCollection
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

        /// <summary>
        /// Renders the provided content as a string.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns></returns>
        Task<string> RenderAsync(IContent content);
    }

    /// <summary>
    /// A renderer for a paragraph.
    /// </summary>
    public class ParagraphRenderer : IContentRenderer
    {
        private readonly ContentRendererCollection _rendererCollection;

        /// <summary>
        /// Initializes a new PragraphRenderer
        /// </summary>
        /// <param name="rendererCollection">The collection of renderer to use for sub-content.</param>
        public ParagraphRenderer(ContentRendererCollection rendererCollection)
        {
            _rendererCollection = rendererCollection;
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
                var renderer = _rendererCollection.GetRendererForContent(subContent);
                sb.Append(renderer.Render(subContent));
            }

            sb.Append("</p>");
            return sb.ToString();
        }

        /// <summary>
        /// Renders the content asynchronously.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns>The rendered string.</returns>
        public Task<string> RenderAsync(IContent content)
        {
            return Task.FromResult(Render(content));
        }
    }

    /// <summary>
    /// A renderer for a heading.
    /// </summary>
    public class HeadingRenderer : IContentRenderer
    {
        private readonly ContentRendererCollection _rendererCollection;

        /// <summary>
        /// Initializes a new HeadingRenderer.
        /// </summary>
        /// <param name="rendererCollection">The collection of renderer to use for sub-content.</param>
        public HeadingRenderer(ContentRendererCollection rendererCollection)
        {
            _rendererCollection = rendererCollection;
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
            return content is Heading1 || content is Heading2 || content is Heading3 || content is Heading4 || content is Heading5 || content is Heading6;
        }

        /// <summary>
        /// Renders the content to an html h-tag.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns>The p-tag as a string.</returns>
        public string Render(IContent content)
        {
            var headingSize = 1;

            switch (content)
            {
                case Heading1 _:
                    break;
                case Heading2 _:
                    headingSize = 2;
                    break;
                case Heading3 _:
                    headingSize = 3;
                    break;
                case Heading4 _:
                    headingSize = 4;
                    break;
                case Heading5 _:
                    headingSize = 5;
                    break;
                case Heading6 _:
                    headingSize = 6;
                    break;
            }

            var heading = content as IHeading;

            var sb = new StringBuilder();
            sb.Append($"<h{headingSize}>");

            foreach (var subContent in heading.Content)
            {
                var renderer = _rendererCollection.GetRendererForContent(subContent);
                sb.Append(renderer.Render(subContent));
            }

            sb.Append($"</h{headingSize}>");
            return sb.ToString();
        }

        /// <summary>
        /// Renders the content asynchronously.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns>The rendered string.</returns>
        public Task<string> RenderAsync(IContent content)
        {
            return Task.FromResult(Render(content));
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

        /// <summary>
        /// Renders the content to a string.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns>The content as a string.</returns>
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
                case "code":
                    return "code";
            }

            return "span";
        }

        /// <summary>
        /// Renders the content asynchronously.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns>The rendered string.</returns>
        public Task<string> RenderAsync(IContent content)
        {
            return Task.FromResult(Render(content));
        }
    }

    /// <summary>
    /// A renderer for an asset.
    /// </summary>
    public class AssetRenderer : IContentRenderer
    {
        private readonly ContentRendererCollection _rendererCollection;

        /// <summary>
        /// Initializes a new AssetRenderer.
        /// </summary>
        /// <param name="rendererCollection">The collection of renderer to use for sub-content.</param>
        public AssetRenderer(ContentRendererCollection rendererCollection)
        {
            _rendererCollection = rendererCollection;
        }

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
            return content is AssetStructure;
        }

        /// <summary>
        /// Renders the content to a string.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns>The html img or a tag.</returns>
        public string Render(IContent content)
        {
            var assetStructure = content as AssetStructure;
            var asset = assetStructure.Data.Target;
            var nodeType = assetStructure.NodeType;
            var sb = new StringBuilder();
            if(nodeType != "asset-hyperlink" && asset.File?.ContentType != null && asset.File.ContentType.ToLower().Contains("image"))
            {
                sb.Append($"<img src=\"{asset.File.Url}\" alt=\"{asset.Title}\" />");
            }else
            {
                sb.Append($"<a href=\"{asset.File.Url}\">");

                if (assetStructure.Content != null && assetStructure.Content.Any())
                {
                    foreach (var subContent in assetStructure.Content)
                    {
                        var renderer = _rendererCollection.GetRendererForContent(subContent);
                        sb.Append(renderer.Render(subContent));
                    }
                }
                else
                {
                    sb.Append(asset.Title);
                }
                sb.Append("</a>");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Renders the content asynchronously.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns>The rendered string.</returns>
        public Task<string> RenderAsync(IContent content)
        {
            return Task.FromResult(Render(content));
        }
    }

    /// <summary>
    /// A renderer for a hyperlink.
    /// </summary>
    public class HyperlinkContentRenderer : IContentRenderer
    {
        private readonly ContentRendererCollection _rendererCollection;

        /// <summary>
        /// Initializes a new HyperlinkContentRenderer.
        /// </summary>
        /// <param name="rendererCollection">The collection of renderer to use for sub-content.</param>
        public HyperlinkContentRenderer(ContentRendererCollection rendererCollection)
        {
            _rendererCollection = rendererCollection;
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

            sb.Append($"<a href=\"{link.Data.Uri}\" title=\"{link.Data.Title}\">");

            foreach (var subContent in link.Content)
            {
                var renderer = _rendererCollection.GetRendererForContent(subContent);
                sb.Append(renderer.Render(subContent));
            }

            sb.Append("</a>");

            return sb.ToString();
        }

        /// <summary>
        /// Renders the content asynchronously.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns>The rendered string.</returns>
        public Task<string> RenderAsync(IContent content)
        {
            return Task.FromResult(Render(content));
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
        public int Order { get; set; } = 500;

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

        /// <summary>
        /// Renders the content asynchronously.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns>The rendered string.</returns>
        public Task<string> RenderAsync(IContent content)
        {
            return Task.FromResult(Render(content));
        }
    }

    /// <summary>
    /// A renderer for a list.
    /// </summary>
    public class ListContentRenderer : IContentRenderer
    {
        private readonly ContentRendererCollection _rendererCollection;

        /// <summary>
        /// Initializes a new ListContentRenderer.
        /// </summary>
        /// <param name="rendererCollection">The collection of renderer to use for sub-content.</param>
        public ListContentRenderer(ContentRendererCollection rendererCollection)
        {
            _rendererCollection = rendererCollection;
        }

        /// <summary>
        /// The order of this renderer in the collection.
        /// </summary>
        public int Order { get; set; } = 100;

        /// <summary>
        /// Renders the content to a string.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns>The list as a ul or ol HTML string.</returns>
        public string Render(IContent content)
        {
            var list = content as List;
            var listTagType = "ul";
            if(list.NodeType == "ordered-list")
            {
                listTagType = "ol";
            }

            var sb = new StringBuilder();

            sb.Append($"<{listTagType}>");

            foreach (var subContent in list.Content)
            {
                var renderer = _rendererCollection.GetRendererForContent(subContent);
                sb.Append(renderer.Render(subContent));
            }

            sb.Append($"</{listTagType}>");

            return sb.ToString();
        }

        /// <summary>
        /// Whether or not this renderer supports the provided content.
        /// </summary>
        /// <param name="content">The content to evaluate.</param>
        /// <returns>Returns true if the content is a list, otherwise false.</returns>
        public bool SupportsContent(IContent content)
        {
            return content is List;
        }

        /// <summary>
        /// Renders the content asynchronously.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns>The rendered string.</returns>
        public Task<string> RenderAsync(IContent content)
        {
            return Task.FromResult(Render(content));
        }
    }

    /// <summary>
    /// A renderer for a list item.
    /// </summary>
    public class ListItemContentRenderer : IContentRenderer
    {
        private readonly ContentRendererCollection _rendererCollection;

        /// <summary>
        /// Initializes a new ListItemContentRenderer.
        /// </summary>
        /// <param name="rendererCollection">The collection of renderer to use for sub-content.</param>
        public ListItemContentRenderer(ContentRendererCollection rendererCollection)
        {
            _rendererCollection = rendererCollection;
        }

        /// <summary>
        /// The order of this renderer in the collection.
        /// </summary>
        public int Order { get; set; } = 100;

        /// <summary>
        /// Renders the content to a string.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns>The list as an li HTML string.</returns>
        public string Render(IContent content)
        {
            var listItem = content as ListItem;

            var sb = new StringBuilder();

            sb.Append($"<li>");

            foreach (var subContent in listItem.Content)
            {
                var renderer = _rendererCollection.GetRendererForContent(subContent);
                sb.Append(renderer.Render(subContent));
            }

            sb.Append($"</li>");

            return sb.ToString();
        }

        /// <summary>
        /// Whether or not this renderer supports the provided content.
        /// </summary>
        /// <param name="content">The content to evaluate.</param>
        /// <returns>Returns true if the content is a list, otherwise false.</returns>
        public bool SupportsContent(IContent content)
        {
            return content is ListItem;
        }

        /// <summary>
        /// Renders the content asynchronously.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns>The rendered string.</returns>
        public Task<string> RenderAsync(IContent content)
        {
            return Task.FromResult(Render(content));
        }
    }

    /// <summary>
    /// A renderer for a quote.
    /// </summary>
    public class QuoteContentRenderer : IContentRenderer
    {
        private readonly ContentRendererCollection _rendererCollection;

        /// <summary>
        /// Initializes a new QuoteContentRenderer.
        /// </summary>
        /// <param name="rendererCollection">The collection of renderer to use for sub-content.</param>
        public QuoteContentRenderer(ContentRendererCollection rendererCollection)
        {
            _rendererCollection = rendererCollection;
        }

        /// <summary>
        /// The order of this renderer in the collection.
        /// </summary>
        public int Order { get; set; } = 100;

        /// <summary>
        /// Renders the content to a string.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns>The list as a quote HTML string.</returns>
        public string Render(IContent content)
        {
            var quote = content as Quote;

            var sb = new StringBuilder();

            sb.Append($"<blockquote>");

            foreach (var subContent in quote.Content)
            {
                var renderer = _rendererCollection.GetRendererForContent(subContent);
                sb.Append(renderer.Render(subContent));
            }

            sb.Append($"</blockquote>");

            return sb.ToString();
        }

        /// <summary>
        /// Whether or not this renderer supports the provided content.
        /// </summary>
        /// <param name="content">The content to evaluate.</param>
        /// <returns>Returns true if the content is a list, otherwise false.</returns>
        public bool SupportsContent(IContent content)
        {
            return content is Quote;
        }

        /// <summary>
        /// Renders the content asynchronously.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns>The rendered string.</returns>
        public Task<string> RenderAsync(IContent content)
        {
            return Task.FromResult(Render(content));
        }
    }

    /// <summary>
    /// A content renderer that renders a horizontal ruler.
    /// </summary>
    public class HorizontalRulerContentRenderer : IContentRenderer
    {
        /// <summary>
        /// The order of this renderer in the collection.
        /// </summary>
        public int Order { get; set; } = 100;

        /// <summary>
        /// Renders the content to a string.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns>An horizontal ruler HTML tag.</returns>
        public string Render(IContent content)
        {
            return "<hr>";
        }

        /// <summary>
        /// Whether or not this renderer supports the provided content.
        /// </summary>
        /// <param name="content">The content to evaluate.</param>
        /// <returns>Returns true if the content is a horizontal ruler, otherwise false.</returns>
        public bool SupportsContent(IContent content)
        {
            return content is HorizontalRuler;
        }

        /// <summary>
        /// Renders the content asynchronously.
        /// </summary>
        /// <param name="content">The content to render.</param>
        /// <returns>The rendered string.</returns>
        public Task<string> RenderAsync(IContent content)
        {
            return Task.FromResult(Render(content));
        }
    }
}
