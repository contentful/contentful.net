using System;

namespace Contentful.Core.Models
{
    /// <summary>
    /// Represents the parameters needed to sign and make requests for embargoed assets.
    /// </summary>
    public class EmbargoedAssetKey
    {
        /// <summary>
        /// The policy parameter to appened to url when requesting an embargoed asset.
        /// </summary>
        public string Policy { get; set; }
        /// <summary>
        /// The secret parameter to appened to a url when requesting an embargoed asset.
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// The date and time after which the policy and secret will no longer be valid.
        /// </summary>
        public DateTime ExpiresAtUtc { get; set; }
    }
}
