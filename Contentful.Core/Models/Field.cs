using Contentful.Core.Models.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contentful.Core.Models
{
    /// <summary>
    /// Represents a field in a <seealso cref="ContentType"/>. Note that this is not the representation of a field with 
    /// actual value in an <seealso cref="Entry{T}"/> or <seealso cref="Asset"/>, but merely a representation of the fields data structure.
    /// </summary>
    public class Field
    {
        /// <summary>
        /// The ID of the field. It must be unique among all fields of the <seealso cref="ContentType"/>.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name of the field. This will be the label of the field in the Contentful web app.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The type of field. Determines what type of data can be stored in the field.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Whether this field is mandatory or not.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Whether this field supports different values for different locales.
        /// </summary>
        public bool Localized { get; set; }

        /// <summary>
        /// The type of link, if any. Normally Asset or Entry.
        /// </summary>
        public string LinkType { get; set; }

        /// <summary>
        /// Whether this field is disabled. Disabled fields are not visible in the Contentful web app.
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Whether this field is omitted in the response from the Content Delivery and Preview APIs.
        /// </summary>
        public bool Omitted { get; set; }

        /// <summary>
        /// Defines a schema for the elements of an array field. Will be null for other types.
        /// </summary>
        public Schema Items { get; set; }

        /// <summary>
        /// The validations that should be applied to the field.
        /// </summary>
        public List<IFieldValidator> Validations { get; set; }
    }
}
