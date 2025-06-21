using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Represents the different types available for the LinkType of a <see cref="Field"/>.
    /// </summary>
    public class SystemLinkTypes
    {
        /// <summary>
        /// A reference to an entry.
        /// </summary>
        public const string Entry = "Entry";
        
        /// <summary>
        /// A reference to an asset.
        /// </summary>
        public const string Asset = "Asset";

        /// <summary>
        /// A reference to a space.
        /// </summary>
        public const string Space = "Space";

        /// <summary>
        /// A reference to a tag.
        /// </summary>
        public const string Tag = "Tag";

        /// <summary>
        /// A reference to a contenttype.
        /// </summary>
        public const string ContentType = "ContentType";

        /// <summary>
        /// A reference to a status object.
        /// </summary>
        public const string Status = "Status";
        
        /// <summary>
        /// An annotation reference for ContentTypes
        /// </summary>
        public const string Annotation = "Annotation";

        /// <summary>
        /// An annotation reference for Environment
        /// </summary>
        public const string Environment = "Environment";

        /// <summary>
        /// A reference to a taxonomy concept.
        /// </summary>
        public const string TaxonomyConcept = "TaxonomyConcept";

        /// <summary>
        /// A reference to a taxonomy concept scheme.
        /// </summary>
        public const string TaxonomyConceptScheme = "TaxonomyConceptScheme";

        /// <summary>
        /// A reference to an entry.
        /// </summary>
        public const string CrossSpaceEntry = "Contentful:Entry";
    }
}