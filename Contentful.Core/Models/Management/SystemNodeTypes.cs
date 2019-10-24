using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{

    /// <summary>
    /// Represents the different types available for a node of a <see cref="IFieldValidator"/>.
    /// </summary>
    public class SystemNodeTypes
    {
        /// <summary>
        /// Heading 1.
        /// </summary>
        public const string BLOCKS_HEADING_1 = "heading-1";

        /// <summary>
        /// Heading 2.
        /// </summary>
        public const string BLOCKS_HEADING_2 = "heading-2";

        /// <summary>
        /// Heading 3.
        /// </summary>
        public const string BLOCKS_HEADING_3 = "heading-3";

        /// <summary>
        /// Heading 4.
        /// </summary>
        public const string BLOCKS_HEADING_4 = "heading-4";

        /// <summary>
        /// Heading 5.
        /// </summary>
        public const string BLOCKS_HEADING_5 = "heading-5";

        /// <summary>
        /// Heading 6.
        /// </summary>
        public const string BLOCKS_HEADING_6 = "heading-6";

        /// <summary>
        /// A paragraph.
        /// </summary>
        public const string BLOCKS_PARAGRAPH = "paragraph";

        /// <summary>
        /// A block quote.
        /// </summary>
        public const string BLOCKS_QUOTE = "blockquote";

        /// <summary>
        /// A horizontal rule.
        /// </summary>
        public const string BLOCKS_HR = "hr";

        /// <summary>
        /// An ordered list.
        /// </summary>
        public const string BLOCKS_OL_LIST = "ordered-list";

        /// <summary>
        /// An unordered list.
        /// </summary>
        public const string BLOCKS_UL_LIST = "unordered-list";

        /// <summary>
        /// A list item.
        /// </summary>
        public const string BLOCKS_LIST_ITEM = "list-item";

        /// <summary>
        /// An embedded entry block.
        /// </summary>
        public const string BLOCKS_EMBEDDED_ENTRY = "embedded-entry-block";

        /// <summary>
        /// An embedded asset block.
        /// </summary>
        public const string BLOCKS_EMBEDDED_ASSET = "embedded-asset-block";

        /// <summary>
        /// An embedded inline entry.
        /// </summary>
        public const string INLINES_EMBEDDED_ENTRY = "embedded-entry-inline";

        /// <summary>
        /// A hyperlink.
        /// </summary>
        public const string INLINES_HYPERLINK = "hyperlink";

        /// <summary>
        /// An asset hyperlink.
        /// </summary>
        public const string INLINES_ASSET_HYPERLINK = "asset-hyperlink";

        /// <summary>
        /// An entry hyperlink.
        /// </summary>
        public const string INLINES_ENTRY_HYPERLINK = "entry-hyperlink";
    }
}
