using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Contentful.Core.Models
{
    /// <summary>
    /// Represents a single asset of a <see cref="Space"/>.
    /// </summary>
    public class Asset : IContentfulResource, IContent
    {
        /// <summary>
        /// Common system managed metadata properties.
        /// </summary>
        [JsonProperty("sys")]
        public SystemProperties SystemProperties { get; set; }

        /// <summary>
        /// The description of the asset.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The title of the asset.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Encapsulates information about the binary file of the asset.
        /// </summary>
        public File File { get; set; }

        /// <summary>
        /// The titles of the asset per locale.
        /// </summary>
        public Dictionary<string, string> TitleLocalized { get; set; }

        /// <summary>
        /// The descriptions of the asset per locale.
        /// </summary>
        public Dictionary<string, string> DescriptionLocalized { get; set; }

        /// <summary>
        /// Information about the file in respective language.
        /// </summary>
        public Dictionary<string, File> FilesLocalized { get; set; }
    }
}
