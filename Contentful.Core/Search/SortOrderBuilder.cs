using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Contentful.Core.Search
{   
    /// <summary>
    /// Utility class to construct a valid sort order parameter for a query to the Contentful API.
    /// </summary>
    public class SortOrderBuilder<T>
    {
        private readonly List<string> _orderList = new List<string>();

        /// <summary>
        /// Initializes a new instance of <see cref="SortOrderBuilder"/>
        /// </summary>
        /// <param name="field">The field to sort by.</param>
        /// <param name="order">The order of the sorting. Default is <see cref="SortOrder.Normal"/>.</param>
        protected SortOrderBuilder(string field, SortOrder order = SortOrder.Normal)
        {
            _orderList.Add($"{(order == SortOrder.Reversed ? "-" : "")}{field}");
        }

        public static SortOrderBuilder<T> New(string field, SortOrder order = SortOrder.Normal)
        {
            return new SortOrderBuilder<T>(field, order);
        }

        public static SortOrderBuilder<T> New<U>(Expression<Func<T, U>> selector, SortOrder order = SortOrder.Normal)
        {
            var memberName = FieldHelpers<T>.GetPropertyName(selector);

            return new SortOrderBuilder<T>(memberName, order);
        }

        /// <summary>
        /// Adds another field to sort by to the current <see cref="SortOrderBuilder"/>.
        /// </summary>
        /// <param name="field">The field to sort by.</param>
        /// <param name="order">The order of the sorting. Default is <see cref="SortOrder.Normal"/>.</param>
        /// <returns>The <see cref="SortOrderBuilder"/> instance.</returns>
        public SortOrderBuilder<T> ThenBy(string field, SortOrder order = SortOrder.Normal)
        {
            _orderList.Add($",{(order == SortOrder.Reversed ? "-" : "")}{field}");
            return this;
        }

        public SortOrderBuilder<T> ThenBy<U>(Expression<Func<T, U>> selector, SortOrder order = SortOrder.Normal)
        {
            var memberName = FieldHelpers<T>.GetPropertyName(selector);

            return ThenBy(memberName, order);
        }

        public string Build()
        {
            return string.Join("", _orderList);
        }
    }
}
