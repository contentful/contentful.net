using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Extensions
{
    public static class JTokenExtensions
    {
        public static bool IsNull(this JToken token)
        {
            return token == null || token.Type == JTokenType.Null;
        }

        public static int ToInt(this JToken token)
        {
            if (token.IsNull())
            {
                return 0;
            }

            return int.Parse(token.ToString());
        }

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
