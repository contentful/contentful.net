using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contentful.Core.Models
{
    /// <summary>
    /// The different types of items you can request a sync for.
    /// </summary>
    public enum SyncType
    {
        /// <summary>
        /// Every type of item.
        /// </summary>
        All,
        /// <summary>
        /// Only assets, excluding deletions.
        /// </summary>
        Asset,
        /// <summary>
        /// Only entries, excluding deletions.
        /// </summary>
        Entry,
        /// <summary>
        /// Only deletions. Both assets and entries deletions are included.
        /// </summary>
        Deletion,
        /// <summary>
        /// Only deletions of assets.
        /// </summary>
        DeletedAsset,
        /// <summary>
        /// Only deletions of entries.
        /// </summary>
        DeletedEntry
    }
}
