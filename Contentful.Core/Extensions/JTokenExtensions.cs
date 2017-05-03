using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Extensions
{
    /// <summary>
    /// Extensionmethods for JToken
    /// </summary>
    public static class JTokenExtensions
    {
        /// <summary>
        /// Checks whether a JToken is null or of null type.
        /// </summary>
        /// <param name="token">The token to validate.</param>
        /// <returns>Whether the token is null or not.</returns>
        public static bool IsNull(this JToken token)
        {
            return token == null || token.Type == JTokenType.Null;
        }

        /// <summary>
        /// Returns an int value from a JToken.
        /// </summary>
        /// <param name="token">The token to retrieve a value from.</param>
        /// <returns>The int value.</returns>
        public static int ToInt(this JToken token)
        {
            if (token.IsNull())
            {
                return 0;
            }

            return int.Parse(token.ToString());
        }

        /// <summary>
        /// Returns a nullable int value from a JToken.
        /// </summary>
        /// <param name="token">The token to retrieve a value from.</param>
        /// <returns>The nullable int value.</returns>
        public static int? ToNullableInt(this JToken token)
        {
            if (token.IsNull())
            {
                return new int?();
            }

            return new int?(token.ToInt());
        }
    }
}
