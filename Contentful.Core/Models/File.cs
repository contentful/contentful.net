using Contentful.Core.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contentful.Core.Models
{
    /// <summary>
    /// Represents information about the actual binary file of an <see cref="Asset"/>.
    /// </summary>
    [JsonConverter(typeof(FileJsonConverter))]
    public class File
    {
        /// <summary>
        /// The original name of the file.
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// The content type of the data contained within this file.
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// An absolute URL to this file.
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// The url to upload this file from.
        /// </summary>
        public string UploadUrl { get; set; }
        /// <summary>
        /// A reference to a SystemProperties metadata object with a type of upload.
        /// </summary>
        public SystemProperties UploadReference { get; set; }
        /// <summary>
        /// Detailed information about the file stored by Contentful.
        /// </summary>
        public FileDetails Details { get; set; }
    }
}
