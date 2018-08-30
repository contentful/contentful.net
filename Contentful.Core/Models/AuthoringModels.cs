using Contentful.Core.Models.Management;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models
{
    /// <summary>
    /// Represents the top level document of a structured text field.
    /// </summary>
    public class Document
    {
        /// <summary>
        /// The type of the node.
        /// </summary>
        public string NodeType { get; set; }

        /// <summary>
        /// The class of the node.
        /// </summary>
        public string NodeClass { get; set; }

        /// <summary>
        /// The list of content this document contains.
        /// </summary>
        public List<IContent> Content { get; set; }
    }

    /// <summary>
    /// Represents a text node of content.
    /// </summary>
    public class Text : IContent
    {
        /// <summary>
        /// The type of the node.
        /// </summary>
        public string NodeType { get; set; }

        /// <summary>
        /// The class of the node.
        /// </summary>
        public string NodeClass { get; set; }

        /// <summary>
        /// The textual value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The marks for this text node.
        /// </summary>
        public List<Mark> Marks { get; set; }
    }

    /// <summary>
    /// Represents a hyperlink content node.
    /// </summary>
    public class Hyperlink : IContent
    {
        /// <summary>
        /// The type of the node.
        /// </summary>
        public string NodeType { get; set; }

        /// <summary>
        /// The class of the node.
        /// </summary>
        public string NodeClass { get; set; }

        /// <summary>
        /// The data of the hyperlink node.
        /// </summary>
        public HyperlinkData Data { get; set; }

        /// <summary>
        /// The list of content this hyperlink contains.
        /// </summary>
        public List<IContent> Content { get; set; }
    }

    /// <summary>
    /// Represents the data of a hyperlink node.
    /// </summary>
    public class HyperlinkData
    {
        /// <summary>
        /// The url of the hyperlink.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The title of the hyperlink.
        /// </summary>
        public string Title { get; set; }
    }

    /// <summary>
    /// Represents a mark for a text node.
    /// </summary>
    public class Mark : IContent
    {
        /// <summary>
        /// The type of mark.
        /// </summary>
        public string Type { get; set; }
    }

    /// <summary>
    /// Represents a paragraph content node.
    /// </summary>
    public class Paragraph : IContent
    {
        /// <summary>
        /// The type of node.
        /// </summary>
        public string NodeType { get; set; }

        /// <summary>
        /// The class of the node.
        /// </summary>
        public string NodeClass { get; set; }

        /// <summary>
        /// The list of content this paragraph contains.
        /// </summary>
        public List<IContent> Content { get; set; }
    }

    /// <summary>
    /// Represents a heading content node.
    /// </summary>
    public class Heading : IContent
    {
        /// <summary>
        /// The type of node.
        /// </summary>
        public string NodeType { get; set; }

        /// <summary>
        /// The class of the node.
        /// </summary>
        public string NodeClass { get; set; }

        /// <summary>
        /// The size of the heading.
        /// </summary>
        public int HeadingSize { get; set; }

        /// <summary>
        /// The list of content this heading contains.
        /// </summary>
        public List<IContent> Content { get; set; }
    }

    /// <summary>
    /// Represents a block content type.
    /// </summary>
    public class Block : IContent
    {
        /// <summary>
        /// The type of node.
        /// </summary>
        public string NodeType { get; set; }

        /// <summary>
        /// The class of the node.
        /// </summary>
        public string NodeClass { get; set; }

        /// <summary>
        /// The data of the block.
        /// </summary>
        public BlockData Data { get; set; }
    }

    /// <summary>
    /// Represents the data of the block.
    /// </summary>
    public class BlockData : IContent
    {
        /// <summary>
        /// The target entry for the block.
        /// </summary>
        public ReferenceProperties Target { get; set; }
    }

    /// <summary>
    /// Interface that marks a class as a possible part of a content tree.
    /// </summary>
    public interface IContent
    {

    }
}
