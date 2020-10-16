using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Represents the different types available for a <see cref="Field"/>.
    /// </summary>
    public class SystemFieldTypes
    {
        /// <summary>
        /// Short text.
        /// </summary>
        public const string Symbol = "Symbol";

        /// <summary>
        /// Long text.
        /// </summary>
        public const string Text = "Text";

        /// <summary>
        /// An integer.
        /// </summary>
        public const string Integer = "Integer";

        /// <summary>
        /// A floating point number.
        /// </summary>
        public const string Number = "Number";

        /// <summary>
        /// A datetime.
        /// </summary>
        public const string Date = "Date";

        /// <summary>
        /// A boolean value.
        /// </summary>
        public const string Boolean = "Boolean";

        /// <summary>
        /// A location field.
        /// </summary>
        public const string Location = "Location";

        /// <summary>
        /// A link to another asset or entry.
        /// </summary>
        public const string Link = "Link";

        /// <summary>
        /// An array of objects.
        /// </summary>
        public const string Array = "Array";

        /// <summary>
        /// An arbitrary json object.
        /// </summary>
        public const string Object = "Object";

        /// <summary>
        /// An rich text document.
        /// </summary>
        public const string RichText = "RichText";

    }
}
