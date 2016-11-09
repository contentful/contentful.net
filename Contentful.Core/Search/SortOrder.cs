using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contentful.Core.Search
{
    /// <summary>
    /// Represents the sort orders supported by the Contentful API.
    /// </summary>
    public enum SortOrder
    {
        /// <summary>
        /// The normal sort order, e.g. A->Z for alphabetical fields.
        /// </summary>
        Normal,
        /// <summary>
        /// The reversed sort order, e.g Z->A for alphabetical fields.
        /// </summary>
        Reversed
    }
}
