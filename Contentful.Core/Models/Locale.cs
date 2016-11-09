using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contentful.Core.Models
{
    /// <summary>
    /// Represents one available locale of a <seealso cref="Space"/>
    /// </summary>
    public class Locale
    {

        /// <summary>
        /// The language code of the locale, e.g. "en-US" or "tlh".
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Whether this locale is the default locale for the current <seealso cref="Space"/>
        /// </summary>
        public bool Default { get; set; }

        /// <summary>
        /// The name of the locale, e.g. "English" or "Klingon".
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The language code of the optional fallback locale. Will be null if not specified.
        /// </summary>
        public string FallbackCode { get; set; }
    }
}
