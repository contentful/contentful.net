using System;
using System.Collections.Generic;
using System.Text;

namespace Contentful.Core.Models.Management
{
    /// <summary>
    /// Represents the transformation of a webhook body.
    /// </summary>
    public class WebhookTransformation
    {
        /// <summary>
        /// The HTTP method the webhook should use.
        /// </summary>
        public HttpMethods Method { get; set; }

        /// <summary>
        /// The content type the webhook should use.
        /// </summary>
        public TransformationContentTypes ContentType { get; set; }

        /// <summary>
        /// The body of the webhook transformation.
        /// </summary>
        public dynamic Body { get; set; }
    }

    public enum HttpMethods
    {
        POST,
        GET,
        PUT,
        PATCH,
        DELETE
    }

    public enum TransformationContentTypes
    {
        ContentfulManagementPlusJson,
        ContentfulManagementPlusJsonAndCharset,
        ApplicationJson,
        ApplicationJsonAndCharset,
        FormEncoded,
        FormEncodedAndCharset
    }
}
