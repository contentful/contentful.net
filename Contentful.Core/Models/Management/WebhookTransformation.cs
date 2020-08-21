using Contentful.Core.Configuration;
using Newtonsoft.Json;
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
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public dynamic Body { get; set; }
    }

    /// <summary>
    /// Enumeration of available HTTP methods for webhook transformations.
    /// </summary>
    [JsonConverter(typeof(WebhookTransformationMethodConverter))]
    public enum HttpMethods
    {
        /// <summary>
        /// POST
        /// </summary>
        POST,
        /// <summary>
        /// GET
        /// </summary>
        GET,
        /// <summary>
        /// PUT
        /// </summary>
        PUT,
        /// <summary>
        /// PATCH
        /// </summary>
        PATCH,
        /// <summary>
        /// DELETE
        /// </summary>
        DELETE
    }

    /// <summary>
    /// Enumeration of available content types for webhook transformations.
    /// </summary>
    [JsonConverter(typeof(WebhookTransformationContenttypeConverter))]
    public enum TransformationContentTypes
    {
        /// <summary>
        /// application/vnd.contentful.management.v1+json
        /// </summary>
        ContentfulManagementPlusJson,
        /// <summary>
        /// application/vnd.contentful.management.v1+json; charset=utf-8
        /// </summary>
        ContentfulManagementPlusJsonAndCharset,
        /// <summary>
        /// application/json
        /// </summary>
        ApplicationJson,
        /// <summary>
        /// application/json; charset=utf-8
        /// </summary>
        ApplicationJsonAndCharset,
        /// <summary>
        /// application/x-www-form-urlencoded
        /// </summary>
        FormEncoded,
        /// <summary>
        /// application/x-www-form-urlencoded; charset=utf-8
        /// </summary>
        FormEncodedAndCharset
    }
}
