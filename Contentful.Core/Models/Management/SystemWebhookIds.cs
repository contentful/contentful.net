using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Represents the different topic types available for webhooks.
    /// </summary>
    public class SystemWebhookTopics
    {
        /// <summary>
        /// Entry topic.
        /// </summary>
        public const string Entry = "Entry";

        /// <summary>
        /// Asset topic.
        /// </summary>
        public const string Asset = "Asset";

        /// <summary>
        /// ContentType topic.
        /// </summary>
        public const string ContentType = "ContentType";
    }

    /// <summary>
    /// Represents the different topic actions types available for webhooks.
    /// </summary>
    public class SystemWebhookActions
    {
        /// <summary>
        /// Create action.
        /// </summary>
        public const string Create = "create";

        /// <summary>
        /// Save action.
        /// </summary>
        public const string Save = "save";

        /// <summary>
        /// Autosave action. This action is triggered when the Contentful web app automatically saves an entry.
        /// </summary>
        public const string AutoSave = "auto_save";

        /// <summary>
        /// Archive action.
        /// </summary>
        public const string Archive = "archive";

        /// <summary>
        /// Unarchive action.
        /// </summary>
        public const string Unarchive = "unarchive";

        /// <summary>
        /// Publish action.
        /// </summary>
        public const string Publish = "publish";

        /// <summary>
        /// Unpublish action.
        /// </summary>
        public const string Unpublish = "unpublish";

        /// <summary>
        /// Delete action.
        /// </summary>
        public const string Delete = "delete";
    }
}
