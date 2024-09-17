using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Contentful.Core.Models.Management;

namespace Contentful.Core.Search
{
    /// <summary>
    /// Utility builder class to construct a correct scheduled action query for the Contentful API.
    /// </summary>
    public class ScheduledActionQueryBuilder
    {
        /// <summary>
        /// The querystring values.
        /// </summary>
        protected readonly List<KeyValuePair<string, string>> _querystringValues = new List<KeyValuePair<string, string>>();

        /// <summary>
        /// Creates a new instance of a ScheduledActionQueryBuilder.
        /// </summary>
        /// <returns>The created <see cref="ScheduledActionQueryBuilder"/>.</returns>
        public static ScheduledActionQueryBuilder New => new ScheduledActionQueryBuilder();

        /// <summary>
        /// Adds a search parameter to restrict the result by environment
        /// </summary>
        /// <param name="contentTypeId">The ID of the environment to restrict by.</param>
        /// <returns>The <see cref="ScheduledActionQueryBuilder"/> instance.</returns>
        public ScheduledActionQueryBuilder EnvironmentIs(string environment)
        {
            if (_querystringValues.Any(c => c.Key == "environment.sys.id"))
            {
                _querystringValues.RemoveAll(c => c.Key == "environment.sys.id");
            }

            _querystringValues.Add(new KeyValuePair<string, string>("environment.sys.id", environment));
            return this;
        }

        /// <summary>
        /// Adds a search parameter to restrict the result by entity id
        /// </summary>
        /// <param name="entityId">The ID of the entiity to restrict by.</param>
        /// <returns>The <see cref="ScheduledActionQueryBuilder"/> instance.</returns>
        public ScheduledActionQueryBuilder EntityIdIs(string entityId)
        {
            _querystringValues.Add(new KeyValuePair<string, string>("entity.sys.id", entityId));
            return this;
        }

        /// <summary>
        /// Adds a search parameter to restrict the result by entity id
        /// </summary>
        /// <param name="entityIds">A collection of ids to restrict to</param>
        /// <returns>The <see cref="ScheduledActionQueryBuilder"/> instance.</returns>
        public ScheduledActionQueryBuilder EntityIdIn(IEnumerable<string> entityIds)
        {
            _querystringValues.Add(new KeyValuePair<string, string>("entity.sys.id", string.Join(",", entityIds)));
            return this;
        }

        /// <summary>
        /// Adds a search parameter to restrict the result by action status
        /// </summary>
        /// <param name="actionStatus">The status to restrict by</param>
        /// <returns>The <see cref="ScheduledActionQueryBuilder"/> instance.</returns>
        public ScheduledActionQueryBuilder StatusIs(ScheduledActionStatus actionStatus)
        {
            _querystringValues.Add(new KeyValuePair<string, string>("sys.status", actionStatus.ToString().ToLower()));
            return this;
        }

        /// <summary>
        /// Adds a search parameter to restrict the result by action status
        /// </summary>
        /// <param name="actionStatuses">The statuses to restrict by</param>
        /// <returns>The <see cref="ScheduledActionQueryBuilder"/> instance.</returns>
        public ScheduledActionQueryBuilder StatusIn(IEnumerable<ScheduledActionStatus> actionStatuses)
        {
            _querystringValues.Add(new KeyValuePair<string, string>("sys.status[in]", string.Join(",", actionStatuses.Select(a => a.ToString().ToLower()))));
            return this;
        }

        /// <summary>
        /// Adds a search parameter to restrict the result by the scheduled for datetime
        /// </summary>
        /// <param name="scheduledFor">The time to restrict by</param>
        /// <returns>The <see cref="ScheduledActionQueryBuilder"/> instance.</returns>
        public ScheduledActionQueryBuilder ScheduledForExactMatch(DateTime scheduledFor)
        {
            _querystringValues.Add(new KeyValuePair<string, string>("scheduledFor.datetime", scheduledFor.ToString("o")));
            return this;
        }

        /// <summary>
        /// Adds a limit to the number of results returned for the query.
        /// </summary>
        /// <param name="limit">The maximum number of hits returned.</param>
        /// <returns>The <see cref="ScheduledActionQueryBuilder"/> instance.</returns>
        public ScheduledActionQueryBuilder Limit(int limit)
        {
            _querystringValues.Add(new KeyValuePair<string, string>("limit", limit.ToString()));
            return this;
        }

        /// <summary>
        /// Adds a search parameter to restrict the result by the scheduled for datetime
        /// </summary>
        /// <param name="scheduledFor">The time to restrict by</param>
        /// <returns>The <see cref="ScheduledActionQueryBuilder"/> instance.</returns>
        public ScheduledActionQueryBuilder ScheduledForLessThan(DateTime scheduledFor)
        {
            return ScheduledForAction(scheduledFor, "lt");
        }

        /// <summary>
        /// Adds a search parameter to restrict the result by the scheduled for datetime
        /// </summary>
        /// <param name="scheduledFor">The time to restrict by</param>
        /// <returns>The <see cref="ScheduledActionQueryBuilder}"/> instance.</returns>
        public ScheduledActionQueryBuilder ScheduledForLessThanOrEqual(DateTime scheduledFor)
        {
            return ScheduledForAction(scheduledFor, "lte");
        }

        /// <summary>
        /// Adds a search parameter to restrict the result by the scheduled for datetime
        /// </summary>
        /// <param name="scheduledFor">The time to restrict by</param>
        /// <returns>The <see cref="ScheduledActionQueryBuilder"/> instance.</returns>
        public ScheduledActionQueryBuilder ScheduledForGreaterThan(DateTime scheduledFor)
        {
            return ScheduledForAction(scheduledFor, "gt");
        }

        /// <summary>
        /// Adds a search parameter to restrict the result by the scheduled for datetime
        /// </summary>
        /// <param name="scheduledFor">The time to restrict by</param>
        /// <returns>The <see cref="ScheduledActionQueryBuilder"/> instance.</returns>
        public ScheduledActionQueryBuilder ScheduledForGreaterThanOrEqual(DateTime scheduledFor)
        {
            return ScheduledForAction(scheduledFor, "gte");
        }

        /// <summary>
        /// Adds a search parameter to restrict the result by the scheduled for datetime
        /// </summary>
        /// <param name="scheduledFor">The time to restrict by</param>
        /// <returns>The <see cref="ScheduledActionQueryBuilder"/> instance.</returns>
        public ScheduledActionQueryBuilder OrderByScheduledFor(bool ascending = true)
        {
            if (ascending)
                return this;

            _querystringValues.Add(new KeyValuePair<string, string>("order", "-scheduledFor.datetime"));
            return this;
        }

        ScheduledActionQueryBuilder ScheduledForAction(DateTime scheduledFor, string operation)
        {
            _querystringValues.Add(new KeyValuePair<string, string>($"scheduledFor.datetime[{operation}]", scheduledFor.ToString("o")));
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
                sb.Append(System.Net.WebUtility.UrlEncode(parameter.Value));
                hasQuery = true;
            }

            return sb.ToString();
        }
    }
}
