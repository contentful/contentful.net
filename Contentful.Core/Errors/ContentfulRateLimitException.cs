using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Errors
{
    public class ContentfulRateLimitException : ContentfulException
    {
        public ContentfulRateLimitException(string message) : base(429, message)
        {
        }

        public int SecondsUntilNextRequest { get; set; }
    }
}
