using Contentful.Core.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
    [JsonConverter(typeof(EditorInterfaceControlJsonConverter))]
    public class EditorInterfaceControl
    {
        public string FieldId { get; set; }
        public string WidgetId { get; set; }
        public EditorInterfaceControlSettings Settings { get; set; }
    }
}
