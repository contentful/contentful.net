using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contentful.Core.Search
{
    /// <summary>
    /// Utility builder class to construct a correct search query for the Contentful API.
    /// </summary>
    public class QueryBuilder
    {
        private readonly List<KeyValuePair<string, string>> _querystringValues = new List<KeyValuePair<string, string>>();

        /// <summary>
        /// Creates a new instance of a querybuilder.
        /// </summary>
        /// <returns>The created <see cref="QueryBuilder"/>.</returns>
        public static QueryBuilder New()
        {
            return new QueryBuilder();
        }

        /// <summary>
        /// Adds a search parameter to restrict the result by content type.
        /// </summary>
        /// <param name="contentTypeId">The ID of the content type to restrict by.</param>
        /// <returns>The <see cref="QueryBuilder"/> instance.</returns>
        public QueryBuilder ContentTypeIs(string contentTypeId)
        {
            _querystringValues.Add(new KeyValuePair<string, string>("content_type", contentTypeId));
            return this;
        }

        /// <summary>
        /// Adds a search parameter to search across all text and symbol fields.
        /// </summary>
        /// <param name="query">The case insensitive query to search for. Has to be at least 2 characters long to be applied.</param>
        /// <returns>The <see cref="QueryBuilder"/> instance.</returns>
        public QueryBuilder FullTextSearch(string query)
        {
            if (!string.IsNullOrEmpty(query) && query.Length >= 2)
            {
                _querystringValues.Add(new KeyValuePair<string, string>("query", query));
            }

            return this;
        }

        /// <summary>
        /// Adds a search parameter on proximity of a coordinate of a location field.
        /// </summary>
        /// <param name="field">The location field to check proximity for.</param>
        /// <param name="coordinate">The coordinate.</param>
        /// <returns>The <see cref="QueryBuilder"/> instance.</returns>
        public QueryBuilder InProximityOf(string field, string coordinate)
        {
            _querystringValues.Add(new KeyValuePair<string, string>($"{field}[near]", coordinate));
            return this;
        }

        /// <summary>
        /// Adds a restriction parameter to only return resources with a location field within the specified area.
        /// </summary>
        /// <param name="field">The location field to check if it is within the bounding box.</param>
        /// <param name="latitude1">The latitude of the bottom left corner of the rectangle.</param>
        /// <param name="longitude1">The longitude of the bottom left corner of the rectangle.</param>
        /// <param name="latitude2">The latitude of the top right corner of the rectangle.</param>
        /// <param name="longitude2">The longitude of the top right corner of the rectangle.</param>
        /// <returns>The <see cref="QueryBuilder"/> instance.</returns>
        public QueryBuilder WithinArea(string field, string latitude1, string longitude1,
            string latitude2, string longitude2)
        {
            _querystringValues.Add(new KeyValuePair<string, string>($"{field}[within]", $"{latitude1},{longitude1},{latitude2},{longitude2}"));
            return this;
        }

        /// <summary>
        /// Adds a restriction parameter to only return resources with a location field within a certain radius of a coordinate.
        /// </summary>
        /// <param name="field">The location field to check if it is within the raidus.</param>
        /// <param name="latitude">The latitude of the centre of the bounding circle.</param>
        /// <param name="longitude">The longitude of the centre of the bounding circle.</param>
        /// <param name="radius">The radius in kilometers of the bounding circle.</param>
        /// <returns>The <see cref="QueryBuilder"/> instance.</returns>
        public QueryBuilder WithinRadius(string field, string latitude, string longitude, float radius)
        {
            _querystringValues.Add(new KeyValuePair<string, string>($"{field}[within]", $"{latitude},{longitude},{radius}"));
            return this;
        }

        /// <summary>
        /// Adds an order expression to the query. An order expression is most simply created using the <see cref="SortOrderBuilder"/> class.
        /// </summary>
        /// <param name="order">The order expression.</param>
        /// <returns>The <see cref="QueryBuilder"/> instance.</returns>
        public QueryBuilder OrderBy(string order)
        {
            _querystringValues.Add(new KeyValuePair<string, string>("order", order));
            return this;
        }

        /// <summary>
        /// Adds a limit to the number of results returned for the query.
        /// </summary>
        /// <param name="limit">The maximum number of hits returned.</param>
        /// <returns>The <see cref="QueryBuilder"/> instance.</returns>
        public QueryBuilder Limit(int limit)
        {
            _querystringValues.Add(new KeyValuePair<string, string>("limit", limit.ToString()));
            return this;
        }

        /// <summary>
        /// Adds a skip to the results returned for the query. 
        /// Use in conjunction with <see cref="OrderBy"/> and <see cref="Limit"/> to effectively page through content.
        /// </summary>
        /// <param name="skip">The number of items skipped.</param>
        /// <returns>The <see cref="QueryBuilder"/> instance.</returns>
        public QueryBuilder Skip(int skip)
        {
            _querystringValues.Add(new KeyValuePair<string, string>("skip", skip.ToString()));
            return this;
        }

        /// <summary>
        /// Select the number of levels of referenced content that should be included in the response.
        /// The default is 1 if this querystring parameter is omitted.
        /// </summary>
        /// <param name="levels">The number of levels of referenced content to include.</param>
        /// <returns>The <see cref="QueryBuilder"/> instance.</returns>
        public QueryBuilder Include(int levels)
        {
            _querystringValues.Add(new KeyValuePair<string, string>("include", levels.ToString()));
            return this;
        }

        /// <summary>
        /// Filters the returned resources by the specified locale.
        /// </summary>
        /// <param name="locale">The locale to filter by.</param>
        /// <returns>The <see cref="QueryBuilder"/> instance.</returns>
        public QueryBuilder LocaleIs(string locale)
        {
            _querystringValues.Add(new KeyValuePair<string, string>("locale", locale));
            return this;
        }

        /// <summary>
        /// Adds a restriction parameter for what mimetypes should be returned by the query.
        /// </summary>
        /// <param name="mimetype">The mimetype to filter by.</param>
        /// <returns>The <see cref="QueryBuilder"/> instance.</returns>
        public QueryBuilder MimeTypeIs(MimeTypeRestriction mimetype)
        {
            _querystringValues.Add(new KeyValuePair<string, string>("mimetype_group", $"{mimetype.ToString().ToLower()}"));
            return this;
        }

        /// <summary>
        /// Adds a restriction that a certain field must exactly match the specified value.
        /// </summary>
        /// <param name="field">The field that must match the value.</param>
        /// <param name="value">The value that the field must exactly match.</param>
        /// <returns>The <see cref="QueryBuilder"/> instance.</returns>
        public QueryBuilder FieldEquals(string field, string value)
        {
            return AddFieldRestriction(field, value, string.Empty);
        }

        /// <summary>
        /// Adds a restriction that a certain field must not exactly match the specified value.
        /// </summary>
        /// <param name="field">The field that must not match the value.</param>
        /// <param name="value">The value that the field must not match.</param>
        /// <returns>The <see cref="QueryBuilder"/> instance.</returns>
        public QueryBuilder FieldDoesNotEqual(string field, string value)
        {
            return AddFieldRestriction(field, value, "[ne]");
        }

        /// <summary>
        /// Adds a restriction that a certain field must exactly match all the specified values. 
        /// Only applicable for array fields.
        /// </summary>
        /// <param name="field">The field that must exactly match all the values.</param>
        /// <param name="value">The values that the field must inlcude all of.</param>
        /// <returns>The <see cref="QueryBuilder"/> instance.</returns>
        public QueryBuilder FieldEqualsAll(string field, IEnumerable<string> values)
        {
            return AddFieldRestriction(field, string.Join(",", values), "[all]");
        }

        /// <summary>
        /// Adds a restriction that a certain field must include at least one of the specified values. 
        /// </summary>
        /// <param name="field">The field that must exactly match at least one of the specified values.</param>
        /// <param name="value">The values that the field must include at least one of.</param>
        /// <returns>The <see cref="QueryBuilder"/> instance.</returns>
        public QueryBuilder FieldIncludes(string field, IEnumerable<string> values)
        {
            return AddFieldRestriction(field, string.Join(",", values), "[in]");
        }

        /// <summary>
        /// Adds a restriction that a certain field must not include any of the specified values. 
        /// </summary>
        /// <param name="field">The field that must not contain any of the specified values.</param>
        /// <param name="value">The values that the field must not include any of.</param>
        /// <returns>The <see cref="QueryBuilder"/> instance.</returns>
        public QueryBuilder FieldExcludes(string field, IEnumerable<string> values)
        {
            return AddFieldRestriction(field, string.Join(",", values), "[nin]");
        }

        /// <summary>
        /// Adds a restriction that a certain field must exist and not be null.
        /// </summary>
        /// <param name="field">The field that must exist and also not be null.</param>
        /// <param name="mustExist">Whether or not the field must exist or not exist. A value of false means only include entries where the particular field does NOT exist.</param>
        /// <returns>The <see cref="QueryBuilder"/> instance.</returns>
        public QueryBuilder FieldExists(string field, bool mustExist = true)
        {
            _querystringValues.Add(new KeyValuePair<string, string>($"{field}[exists]", mustExist.ToString().ToLower()));
            return this;
        }

        /// <summary>
        /// Adds a restriction that a certain field must be less than the specified value.
        /// </summary>
        /// <param name="field">The field to compare against.</param>
        /// <param name="value">The value the field must be less than.</param>
        /// <returns>The <see cref="QueryBuilder"/> instance.</returns>
        public QueryBuilder FieldLessThan(string field, string value)
        {
            return AddFieldRestriction(field, value, "[lt]");
        }

        /// <summary>
        /// Adds a restriction that a certain field must be less than or equal to the specified value.
        /// </summary>
        /// <param name="field">The field to compare against.</param>
        /// <param name="value">The value the field must be less than or equal to.</param>
        /// <returns>The <see cref="QueryBuilder"/> instance.</returns>
        public QueryBuilder FieldLessThanOrEqualTo(string field, string value)
        {
            return AddFieldRestriction(field, value, "[lte]");
        }

        /// <summary>
        /// Adds a restriction that a certain field must be greater than the specified value.
        /// </summary>
        /// <param name="field">The field to compare against.</param>
        /// <param name="value">The value the field must be greater than.</param>
        /// <returns>The <see cref="QueryBuilder"/> instance.</returns>
        public QueryBuilder FieldGreaterThan(string field, string value)
        {
            return AddFieldRestriction(field, value, "[gt]");
        }

        /// <summary>
        /// Adds a restriction that a certain field must be greater than or equal to the specified value.
        /// </summary>
        /// <param name="field">The field to compare against.</param>
        /// <param name="value">The value the field must be greater than or equal to.</param>
        /// <returns>The <see cref="QueryBuilder"/> instance.</returns>
        public QueryBuilder FieldGreaterThanOrEqualTo(string field, string value)
        {
            return AddFieldRestriction(field, value, "[gte]");
        }

        /// <summary>
        /// Adds a search parameter to search in a specific field for a match for a value. 
        /// Not to be confused with the <see cref="FullTextSearch"/> method that searches across all fields.
        /// </summary>
        /// <param name="field">The field to search for matches.</param>
        /// <param name="value">The value the field must match.</param>
        /// <returns>The <see cref="QueryBuilder"/> instance.</returns>
        public QueryBuilder FieldMatches(string field, string value)
        {
            return AddFieldRestriction(field, value, "[match]");
        }

        private QueryBuilder AddFieldRestriction(string field, string value, string @operator)
        {
            _querystringValues.Add(new KeyValuePair<string, string>($"{field}{@operator}", value));
            return this;
        }

        /// <summary>
        /// Builds the query and returns the formatted querystring.
        /// </summary>
        /// <returns>The formatted querystring.</returns>
        public string Build()
        {
            var sb = new StringBuilder();
            var hasQuery = false;

            foreach (var parameter in _querystringValues)
            {
                sb.Append(hasQuery ? '&' : '?');
                sb.Append(parameter.Key);
                sb.Append('=');
                sb.Append(parameter.Value);
                hasQuery = true;
            }

            return sb.ToString();
        }
    }
}
