using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
    public class WebHookRequest
    {
        public string Url { get; set; }
        public List<KeyValuePair<string, string>> Headers { get; set; }
        public string Body { get; set; }
        public string Method { get; set; }
        public int Timeout { get; set; }
    }
}
