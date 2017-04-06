using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Represents the available scopes of management tokens.
    /// </summary>
    public class SystemManagementScopes
    {
        /// <summary>
        /// Allows read only access to the management API.
        /// </summary>
        public const string Read = "content_management_read";

        /// <summary>
        /// Allows full access to the management API.
        /// </summary>
        public const string Manage = "content_management_manage";
    }
}
