using System;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Represents the different units available for <see cref="FileSizeValidator"/>.
    /// </summary>
    public class SystemFileSizeUnits
    {
        /// <summary>
        /// Measure the file in bytes.
        /// </summary>
        public const string Bytes = "Bytes";

        /// <summary>
        /// Measure the file in KB.
        /// </summary>
        public const string KB = "KB";

        /// <summary>
        /// Measure the file in MB.
        /// </summary>
        public const string MB = "MB";
    }
}
