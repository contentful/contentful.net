using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
    public class WebHookResponse
    {
        public string Url { get; set; }
        public Dictionary<string,dynamic> Headers { get; set; }
        public string Body { get; set; }
        public int StatusCode { get; set; }
    }
}
