using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Contentful.Core.Search
{
    public static class ExpressionHelpers
    {
        public static string ToCamelCase(this string original)
        {
            if (string.IsNullOrWhiteSpace(original))
            {
                return string.Empty;
            }

            if (original.ToUpperInvariant() == original)
            {
                // is all uppercase, bail as acronym
                return original;
            }

            // will check each character in order and the first one that is lower will engage fail-fast which accumulates speed.
            // in testing this massively outperforms regex.replace and StringBuilder.Append with bounded limits (the way to concatenate lower change + existing remainder w/o substring)
            bool isAtBeginning = true;
            string converted = new(original.Select(s =>
            {
                if (!isAtBeginning)
                {
                    return s;
                }

                if (char.IsUpper(s) && isAtBeginning)
                {
                    return char.ToLowerInvariant(s);
                }

                if (char.IsLower(s) && isAtBeginning)
                {
                    isAtBeginning = false;
                }
                return s;
            }).ToArray());
            return converted;
        }
    }
}
