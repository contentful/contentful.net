using Contentful.Core.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Represents how a specific field of a <see cref="ContentType"/> should be represented visually.
    /// </summary>
    public class EditorLayoutGroup: IEditorLayoutGroupItem
    {
        /// <summary>
        /// The group identifier of the group
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// The display name of the group
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The widget namespace.
        /// </summary>
        public List<IEditorLayoutGroupItem> Items { get; set; }
    }

    /// <summary>
    /// Represents a field for a <see cref="EditorLayoutGroup"/>
    /// </summary>
    public class EditorLayoutFieldItem: IEditorLayoutGroupItem
    {
        /// <summary>
        /// Unique identifier of the field, it must be a field id in the related content type
        /// </summary>
        public string FieldId { get; set; }
    }

    /// <summary>
    /// Interface to group <see cref="EditorLayoutFieldItem"/> and <see cref="EditorLayoutGroup"/>
    /// </summary>
    public interface IEditorLayoutGroupItem { }
}
