using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models
{
    /// <summary>
    /// Encapsulates the settings available to set for each separate space you wish to resolve cross space references from.
    /// </summary>
    public class CrossSpaceResolutionSetting
    {
        /// <summary>
        /// The Space Id of the space.
        /// </summary>
        public string SpaceId { get; set; }

        /// <summary>
        /// The CDA token for the space.
        /// </summary>
        public string CdaToken { get; set; }
    }
}
