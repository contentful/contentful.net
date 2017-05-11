using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Represents a user of the content management api.
    /// </summary>
    public class User : IContentfulResource
    {
        /// <summary>
        /// Common system managed metadata properties.
        /// </summary>
        [JsonProperty("sys")]
        public SystemProperties SystemProperties { get; set; }

        /// <summary>
        /// The first name of the user.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the user.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// The url of the users avatar.
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// The email address of the user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Whether or not the user is activated.
        /// </summary>
        public bool Activated { get; set; }

        /// <summary>
        /// The number of times the user has signed in.
        /// </summary>
        public int SignInCount { get; set; }

        /// <summary>
        /// Whether or not the user is confirmed.
        /// </summary>
        public bool Confirmed { get; set; }
    }
}
