using Contentful.Core.Models.Management;
using Newtonsoft.Json.Linq;
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
        /// The data for the document node.
        /// </summary>
        public EmptyNodeData Data { get; set; }

        /// <summary>
        /// The type of the node.
        /// </summary>
        public string NodeType { get; set; }

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
        /// The data for the text node.
        /// </summary>
        public EmptyNodeData Data { get; set; }

        /// <summary>
        /// The type of the node.
        /// </summary>
        public string NodeType { get; set; }

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
        public string Uri { get; set; }

        /// <summary>
        /// The title of the hyperlink.
        /// </summary>
        public string Title { get; set; }
    }

    /// <summary>
    /// Represents a mark for a text node.
    /// </summary>
    public class Mark
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
        /// The data for the paragraph node.
        /// </summary>
        public EmptyNodeData Data { get; set; }

        /// <summary>
        /// The type of node.
        /// </summary>
        public string NodeType { get; set; }

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
        /// The data for the heading node.
        /// </summary>
        public EmptyNodeData Data { get; set; }

        /// <summary>
        /// The type of node.
        /// </summary>
        public string NodeType { get; set; }

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
    /// Represents a list content node.
    /// </summary>
    public class List : IContent
    {
        /// <summary>
        /// The data for the list node.
        /// </summary>
        public EmptyNodeData Data { get; set; }

        /// <summary>
        /// The type of node.
        /// </summary>
        public string NodeType { get; set; }

        /// <summary>
        /// The list of content this paragraph contains.
        /// </summary>
        public List<IContent> Content { get; set; }
    }

    /// <summary>
    /// Represents a list item content node.
    /// </summary>
    public class ListItem : IContent
    {
        /// <summary>
        /// The data for the list item node.
        /// </summary>
        public EmptyNodeData Data { get; set; }

        /// <summary>
        /// The type of node.
        /// </summary>
        public string NodeType { get; set; }

        /// <summary>
        /// The list of content this paragraph contains.
        /// </summary>
        public List<IContent> Content { get; set; }
    }

    /// <summary>
    /// Represents a quote content node.
    /// </summary>
    public class Quote : IContent
    {
        /// <summary>
        /// The data for the quote node.
        /// </summary>
        public EmptyNodeData Data { get; set; }

        /// <summary>
        /// The type of node.
        /// </summary>
        public string NodeType { get; set; }

        /// <summary>
        /// The list of content this paragraph contains.
        /// </summary>
        public List<IContent> Content { get; set; }
    }

    /// <summary>
    /// Represents a horizontal ruler content node.
    /// </summary>
    public class HorizontalRuler : IContent
    {
        /// <summary>
        /// The data for the heading node.
        /// </summary>
        public EmptyNodeData Data { get; set; }

        /// <summary>
        /// The type of node.
        /// </summary>
        public string NodeType { get; set; }
    }

    /// <summary>
    /// Represents an hyperlink to an asset.
    /// </summary>
    public class AssetHyperlink : IContent
    {
        /// <summary>
        /// The type of node.
        /// </summary>
        public string NodeType { get; set; }

        /// <summary>
        /// The list of content this asset contains.
        /// </summary>
        public List<IContent> Content { get; set; }

        /// <summary>
        /// The data of the asset hyperlink node.
        /// </summary>
        public AssetHyperlinkData Data { get; set; }
    }

    /// <summary>
    /// Represents the data of the asset hyperlink.
    /// </summary>
    public class AssetHyperlinkData
    {
        /// <summary>
        /// The asset this hyperlink targets
        /// </summary>
        public Asset Target { get; set; }
    }

    /// <summary>
    /// Represents an hyperlink to an entry.
    /// </summary>
    public class EntryStructure : IContent
    {
        /// <summary>
        /// The type of node.
        /// </summary>
        public string NodeType { get; set; }

        /// <summary>
        /// The list of content this asset contains.
        /// </summary>
        public List<IContent> Content { get; set; }

        /// <summary>
        /// The data of the entry hyperlink node.
        /// </summary>
        public EntryStructureData Data { get; set; }
    }

    /// <summary>
    /// Represents the data of the entry hyperlink.
    /// </summary>
    public class EntryStructureData
    {
        /// <summary>
        /// The entry this hyperlink targets
        /// </summary>
        public IContent Target { get; set; }
    }

    /// <summary>
    /// Represents an hyperlink to an asset.
    /// </summary>
    public class AssetStructure : IContent
    {
        /// <summary>
        /// The type of node.
        /// </summary>
        public string NodeType { get; set; }

        /// <summary>
        /// The list of content this asset contains.
        /// </summary>
        public List<IContent> Content { get; set; }

        /// <summary>
        /// The data of the asset hyperlink node.
        /// </summary>
        public AssetStructureData Data { get; set; }
    }

    /// <summary>
    /// Represents the data of the asset hyperlink.
    /// </summary>
    public class AssetStructureData
    {
        /// <summary>
        /// The asset this hyperlink targets
        /// </summary>
        public Asset Target { get; set; }
    }

    /// <summary>
    /// Represents a custom created content node.
    /// </summary>
    public class CustomNode : IContent
    {
        /// <summary>
        /// The data for the custom node.
        /// </summary>
        public EmptyNodeData Data { get; set; }

        /// <summary>
        /// The JSON data of the node.
        /// </summary>
        public JObject JObject { get; set; }
    }

    /// <summary>
    /// Represents an empty node data structure for nodes where the data property is empty.
    /// </summary>
    public class EmptyNodeData
    {
    }

    /// <summary>
    /// Interface that marks a class as a possible part of a content tree.
    /// </summary>
    public interface IContent
    {

    }
}
