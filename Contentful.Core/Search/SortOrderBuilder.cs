using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contentful.Core.Search
{
    /// <summary>
    /// Utility class to construct a valid sort order parameter for a query to the Contentful API.
    /// </summary>
    public class SortOrderBuilder
    {
        private readonly List<string> _orderList = new List<string>();

        /// <summary>
        /// Initializes a new instance of <see cref="SortOrderBuilder"/>
        /// </summary>
        /// <param name="field">The field to sort by.</param>
        /// <param name="order">The order of the sorting. Default is <see cref="SortOrder.Normal"/>.</param>
        public SortOrderBuilder(string field, SortOrder order = SortOrder.Normal)
        {
            _orderList.Add($"{(order == SortOrder.Reversed ? "-" : "")}{field}");
        }

        /// <summary>
        /// Adds another field to sort by to the current <see cref="SortOrderBuilder"/>.
        /// </summary>
        /// <param name="field">The field to sort by.</param>
        /// <param name="order">The order of the sorting. Default is <see cref="SortOrder.Normal"/>.</param>
        /// <returns>The <see cref="SortOrderBuilder"/> instance.</returns>
        public SortOrderBuilder ThenBy(string field, SortOrder order = SortOrder.Normal)
        {
            _orderList.Add($",{(order == SortOrder.Reversed ? "-" : "")}{field}");
            return this;
        }

        public string Build()
        {
            return string.Join("", _orderList);
        }
    }
}
