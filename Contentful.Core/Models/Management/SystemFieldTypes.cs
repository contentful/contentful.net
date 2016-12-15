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
        public const string Symbol = "Symbol";
        public const string Text = "Text";
        public const string Integer = "Integer";
        public const string Number = "Number";
        public const string Date = "Date";
        public const string Boolean = "Boolean";
        public const string Location = "Location";
        public const string Link = "Link";
        public const string Array = "Array";
        public const string Object = "Object";

    }
}
