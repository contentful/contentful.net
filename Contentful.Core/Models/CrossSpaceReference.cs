using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models
{
    /// <summary>
    /// Represents a reference link to a different space.
    /// Allows you to easily model reference fields to cross space entries when creating new entries.
    /// </summary>
    public class CrossSpaceReference
    {
        /// <summary>
        /// The properties for this reference.
        /// </summary>
        public CrossSpaceReferenceProperties Sys { get; set; }
    }

    /// <summary>
    /// Encapsulates the three properties that a <see cref="CrossSpaceReference"/> consists of.
    /// </summary>
    public class CrossSpaceReferenceProperties
    {
        /// <summary>
        /// The type of object, for cross space references this should always be "ResourceLink".
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The type of link. Normally one of <see cref="Management.SystemLinkTypes"/>. For cross space
        /// entry references would be "Contentful:Entry"
        /// </summary>
        public string LinkType { get; set; }
        /// <summary>
        /// The id of the item which is being referenced.
        /// </summary>
        public string Urn { get; set; }
    }
}
