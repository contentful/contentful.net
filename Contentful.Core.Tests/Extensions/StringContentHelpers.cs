using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contentful.Core.Tests.Extensions
{
    public class StringContentHelpers
    {
        public static string Encoded(string key, string value)
        {
            return $"{key}={System.Net.WebUtility.UrlEncode(value)}";
        }
    }
}
