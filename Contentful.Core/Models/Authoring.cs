using Contentful.Core.Models.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contentful.Core.Models
{
    public class Document
    {
        public string Type { get; set; }
        public string Category { get; set; }
        public List<IContent> Content { get; set; }
    }

    public class Text : IContent
    {
        public string Type { get; set; }
        public string Category { get; set; }
        public string Value { get; set; }
        public List<Mark> Marks { get; set; }
    }

    public class Mark : IContent
    {
        public string Type { get; set; }
    }

    public class Paragraph : IContent
    {
        public string Type { get; set; }
        public string Category { get; set; }
        public List<IContent> Content { get; set; }
    }

    public class Block : IContent
    {
        public string Type { get; set; }
        public string Category { get; set; }
        public ReferenceProperties Sys { get; set; }
    }

    public interface IContent
    {

    }

    public class HtmlRenderer
    {
        private readonly ContentRenderererCollection _contentRenderererCollection;

        public HtmlRenderer(ContentRenderererCollection contentRenderererCollection)
        {
            _contentRenderererCollection = contentRenderererCollection;
        }

        public  string ToHtml(Document doc)
        {
            var sb = new StringBuilder();
            foreach (var content in doc.Content)
            {
                var renderer = _contentRenderererCollection.GetRendererForContent(content);
                sb.Append(renderer.Render(content));
            }

            return sb.ToString();
        }
    }

    public class ContentRenderererCollection
    {
        readonly List<IContentRenderer> _renderers = new List<IContentRenderer>();
        public void AddRenderer(IContentRenderer renderer)
        {
            _renderers.Add(renderer);
        }

        public IContentRenderer GetRendererForContent(IContent content)
        {
            return _renderers.OrderBy(c => c.Order).FirstOrDefault(c => c.SupportsContent(content));
        }
    }

    public interface IContentRenderer
    {
        int Order { get; set; }
        bool SupportsContent(IContent content);
        string Render(IContent content);
    }

    public class ParagraphRenderer : IContentRenderer
    {
        private readonly ContentRenderererCollection _renderererCollection;

        public ParagraphRenderer(ContentRenderererCollection renderererCollection)
        {
            _renderererCollection = renderererCollection;
        }

        public int Order { get; set; }
        public bool SupportsContent(IContent content)
        {
            return content is Paragraph;
        }
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

    public class TextRenderer : IContentRenderer
    {
        public int Order { get; set; }

        public bool SupportsContent(IContent content)
        {
            return content is Text;
        }

        public string Render(IContent content)
        {
            var text = content as Text;
            var sb = new StringBuilder();

            foreach (var mark in text.Marks)
            {
                sb.Append($"<{MarkToHtmlTag(mark)}>");
            }
            sb.Append(text.Value);

            foreach (var mark in text.Marks)
            {
                sb.Append($"</{MarkToHtmlTag(mark)}>");
            }

            return sb.ToString();
        }

        private string MarkToHtmlTag(Mark mark)
        {
            switch (mark.Type)
            {
                case "bold":
                    return "strong";
            }

            return "span";
        }
    }

    public class AssetRenderer : IContentRenderer
    {
        public int Order { get; set; }

        public bool SupportsContent(IContent content)
        {
            return content is Asset;
        }

        public string Render(IContent content)
        {
            var asset = content as Asset;
            var sb = new StringBuilder();
            if(asset.File?.ContentType?.ToLower() == "image/png")
            {
                sb.Append($"<img src=\"{asset.File.Url}\" alt=\"{asset.Title}\" />");
            }else
            {
                sb.Append($"<a href=\"{asset.File.Url}\">{asset.Title}</a>");
            }

            return sb.ToString();
        }
    }

    public class NullContentRenderer : IContentRenderer
    {
        public int Order { get; set; }
        public string Render(IContent content)
        {
            return "";
        }
        public bool SupportsContent(IContent content)
        {
            return true;
        }
    }
}
