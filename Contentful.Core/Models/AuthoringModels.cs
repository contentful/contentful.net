using Contentful.Core.Models.Management;
using Newtonsoft.Json;
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
        /// The type of the node.
        /// </summary>
        public string NodeType { get; set; }

        /// <summary>
        /// The additional data of the node.
        /// </summary>
        public GenericStructureData Data { get; set; }

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
        /// The additional data of the node.
        /// </summary>
        public GenericStructureData Data { get; set; }

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
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
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
        /// The additional data of the node.
        /// </summary>
        public GenericStructureData Data { get; set; }

        /// <summary>
        /// The list of content this paragraph contains.
        /// </summary>
        public List<IContent> Content { get; set; }
    }

    /// <summary>
    /// Represents a heading1 content node.
    /// </summary>
    public class Heading1 : IContent, IHeading
    {
        /// <summary>
        /// The type of node.
        /// </summary>
        public string NodeType { get; set; }

        /// <summary>
        /// The additional data of the node.
        /// </summary>
        public GenericStructureData Data { get; set; }

        /// <summary>
        /// The list of content this heading contains.
        /// </summary>
        public List<IContent> Content { get; set; }
    }

    /// <summary>
    /// Represents a heading2 content node.
    /// </summary>
    public class Heading2 : IContent, IHeading
    {
        /// <summary>
        /// The type of node.
        /// </summary>
        public string NodeType { get; set; }

        /// <summary>
        /// The additional data of the node.
        /// </summary>
        public GenericStructureData Data { get; set; }

        /// <summary>
        /// The list of content this heading contains.
        /// </summary>
        public List<IContent> Content { get; set; }
    }

    /// <summary>
    /// Represents a heading3 content node.
    /// </summary>
    public class Heading3 : IContent, IHeading
    {
        /// <summary>
        /// The type of node.
        /// </summary>
        public string NodeType { get; set; }

        /// <summary>
        /// The additional data of the node.
        /// </summary>
        public GenericStructureData Data { get; set; }

        /// <summary>
        /// The list of content this heading contains.
        /// </summary>
        public List<IContent> Content { get; set; }
    }

    /// <summary>
    /// Represents a heading4 content node.
    /// </summary>
    public class Heading4 : IContent, IHeading
    {
        /// <summary>
        /// The type of node.
        /// </summary>
        public string NodeType { get; set; }

        /// <summary>
        /// The additional data of the node.
        /// </summary>
        public GenericStructureData Data { get; set; }

        /// <summary>
        /// The list of content this heading contains.
        /// </summary>
        public List<IContent> Content { get; set; }
    }

    /// <summary>
    /// Represents a heading5 content node.
    /// </summary>
    public class Heading5 : IContent, IHeading
    {
        /// <summary>
        /// The type of node.
        /// </summary>
        public string NodeType { get; set; }

        /// <summary>
        /// The additional data of the node.
        /// </summary>
        public GenericStructureData Data { get; set; }

        /// <summary>
        /// The list of content this heading contains.
        /// </summary>
        public List<IContent> Content { get; set; }
    }

    /// <summary>
    /// Represents a heading6 content node.
    /// </summary>
    public class Heading6 : IContent, IHeading
    {
        /// <summary>
        /// The type of node.
        /// </summary>
        public string NodeType { get; set; }

        /// <summary>
        /// The additional data of the node.
        /// </summary>
        public GenericStructureData Data { get; set; }

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
        public Reference Target { get; set; }
    }

    /// <summary>
    /// Represents a list content node.
    /// </summary>
    public class List : IContent
    {
        /// <summary>
        /// The type of node.
        /// </summary>
        public string NodeType { get; set; }

        /// <summary>
        /// The additional data of the node.
        /// </summary>
        public GenericStructureData Data { get; set; }

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
        /// The type of node.
        /// </summary>
        public string NodeType { get; set; }

        /// <summary>
        /// The additional data of the node.
        /// </summary>
        public GenericStructureData Data { get; set; }

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
        /// The type of node.
        /// </summary>
        public string NodeType { get; set; }

        /// <summary>
        /// The additional data of the node.
        /// </summary>
        public GenericStructureData Data { get; set; }

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
        /// The type of node.
        /// </summary>
        public string NodeType { get; set; }

        /// <summary>
        /// This property is added although a horizontal ruler can't really contain any content, but 
        /// some client side libraries depend on the property to exist.
        /// It is not used by the renderer.
        /// </summary>
        public List<IContent> Content { get; set; }

        /// <summary>
        /// The additional data of the node.
        /// </summary>
        public GenericStructureData Data { get; set; }
    }

    /// <summary>
    /// Represents a table content node.
    /// </summary>
    public class Table : IContent
    {
        /// <summary>
        /// The type of node.
        /// </summary>
        public string NodeType { get; set; }

        /// <summary>
        /// The content of the table
        /// </summary>
        public List<IContent> Content { get; set; }

        /// <summary>
        /// The additional data of the node.
        /// </summary>
        public GenericStructureData Data { get; set; }
    }

    /// <summary>
    /// Represents a row of a table content node.
    /// </summary>
    public class TableRow : IContent
    {
        /// <summary>
        /// The type of node.
        /// </summary>
        public string NodeType { get; set; }

        /// <summary>
        /// The content of the table row
        /// </summary>
        public List<IContent> Content { get; set; }

        /// <summary>
        /// The additional data of the node.
        /// </summary>
        public GenericStructureData Data { get; set; }
    }

    /// <summary>
    /// Represents a header of a table content node.
    /// </summary>
    public class TableHeader : IContent
    {
        /// <summary>
        /// The type of node.
        /// </summary>
        public string NodeType { get; set; }

        /// <summary>
        /// The content of the table row
        /// </summary>
        public List<IContent> Content { get; set; }

        /// <summary>
        /// The additional data of the TableHeader. Can be used to set Rowspan and Colspan properties of cell
        /// </summary>
        public TableCellData Data { get; set; }
    }

    /// <summary>
    /// Represents a cell of a table row content node.
    /// </summary>
    public class TableCell : IContent
    {
        /// <summary>
        /// The type of node.
        /// </summary>
        public string NodeType { get; set; }

        /// <summary>
        /// The content of the table cell
        /// </summary>
        public List<IContent> Content { get; set; }

        /// <summary>
        /// The additional data of the TableCell. Can be used to set Rowspan and Colspan properties of cell
        /// </summary>
        public TableCellData Data { get; set; }
    }

    /// <summary>
    /// Represents a TableCell or TableHeader's additional data
    /// </summary>
    public class TableCellData
    {
        /// <summary>
        /// The Rowspan of the cell
        /// </summary>
        public int? Rowspan { get; set; }
        
        /// <summary>
        /// The Colspan of the cell
        /// </summary>
        public int? Colspan { get; set; }
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
    /// Represents the data property of the generic node tpes
    /// </summary>
    public class GenericStructureData
    {
        /// <summary>
        /// The entry this hyperlink targets
        /// </summary>
        public object Target { get; set; }
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
        /// The JSON data of the node.
        /// </summary>
        public JObject JObject { get; set; }
    }

    /// <summary>
    /// Interface that marks a class as a possible part of a content tree.
    /// </summary>
    public interface IContent
    {

    }

    /// <summary>
    /// Interface that marks an IContent as a heading.
    /// </summary>
    public interface IHeading
    {
        /// <summary>
        /// The list of content this heading contains.
        /// </summary>
        List<IContent> Content { get; set; }
    }
}
