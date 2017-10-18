using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Represents a reference link returned from the Contentful API.
    /// Allows you to easily model reference fields when creating new entries.
    /// </summary>
    public class Reference
    {
        /// <summary>
        /// Initializes a new Reference.
        /// </summary>
        public Reference()
        {

        }

        /// <summary>
        /// Initializes a new Reference.
        /// </summary>
        /// <param name="linkType">The linktype of the reference. Normally one of <see cref="SystemLinkTypes"/>.</param>
        /// <param name="id">The id of the item which is being referenced.</param>
        public Reference(string linkType, string id)
        {
            Sys = new ReferenceProperties
            {
                LinkType = linkType,
                Id = id
            };
        }

        /// <summary>
        /// The properties for this reference.
        /// </summary>
        public ReferenceProperties Sys { get; set; }
    }

    /// <summary>
    /// Encapsulates the three properties that a <see cref="Reference"/> consists of.
    /// </summary>
    public class ReferenceProperties
    {
        /// <summary>
        /// The type of object, for references this is always "Link".
        /// </summary>
        public string Type => "Link";

        /// <summary>
        /// The type of link. Normally one of <see cref="SystemLinkTypes"/>.
        /// </summary>
        public string LinkType { get; set; }

        /// <summary>
        /// The id of the item which is being referenced.
        /// </summary>
        public string Id { get; set; }
    }
}
