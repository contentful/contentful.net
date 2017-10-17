using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
    public class Reference
    {
        public string Sys { get; set; }
    }

    public class ReferenceProperties
    {
        public string Type => "Link";
        public string LinkType { get; set; }
        public string Id { get; set; }
    }
}
