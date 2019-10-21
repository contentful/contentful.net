using Contentful.Core.Configuration;
using Contentful.Core.Errors;
using Contentful.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Contentful.Core
{
    /// <summary>
    /// Base class for Contentful clients.
    /// </summary>
    public abstract class ContentfulClientBase
    {
        /// <summary>
        /// The HttpClient used for API calls.
        /// </summary>
        protected HttpClient _httpClient;

        /// <summary>
        /// The <see cref="ContentfulOptions"/> for this ContentfulClient.
        /// </summary>
        protected ContentfulOptions _options;

        internal string EnvironmentsBase => string.IsNullOrEmpty(_options.Environment) ? "" : $"environments/{_options.Environment}/";

        internal JsonSerializer Serializer => JsonSerializer.Create(SerializerSettings);

        /// <summary>
        /// Gets or sets the settings that should be used for deserialization.
        /// </summary>
        public JsonSerializerSettings SerializerSettings { get; set; } = new JsonSerializerSettings();

        /// <summary>
        /// Returns the current version of the package.
        /// </summary>
        public string Version => typeof(ContentfulClientBase).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            .InformationalVersion;

        private string Os => IsWindows ? "Windows" : IsMacOS ? "macOS" : "Linux";

        private string Platform => ".net";

        private static bool IsWindows
        {
            get
            {
                try
                {
                    return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                }
                catch (PlatformNotSupportedException) { }
                return false;
            }
        }

        private static bool IsMacOS
        {
            get
            {
                try
                {
                    return RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
                }
                catch (PlatformNotSupportedException) { }
                return false;
            }
        }

        /// <summary>
        /// Property for sending a custom tracking header.
        /// </summary>
        public string Application { get; set; } = "sdk contentful.csharp";

        /// <summary>
        /// Creates an exception for a failed API request.
        /// </summary>
        /// <param name="res">The HttpResonseMessage.</param>
        /// <returns></returns>
        protected async Task CreateExceptionForFailedRequest(HttpResponseMessage res)
        {
            var responseContent = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
            var jsonError = string.IsNullOrEmpty(responseContent) ? null : JObject.Parse(responseContent);
            var sys = jsonError?.SelectToken("$.sys").ToObject<SystemProperties>();
            var errorDetails = jsonError?.SelectToken("$.details")?.ToObject<ErrorDetails>();
            var message = jsonError?.SelectToken("$.message")?.ToString();
            var statusCode = (int)res.StatusCode;

            if (string.IsNullOrEmpty(message))
            {
                message = GetGenericErrorMessageForStatusCode(statusCode, sys?.Id);
            }

            if(errorDetails != null)
            {
                message += errorDetails.Errors?.ToString();
            }

            IEnumerable<string> headers = new List<string>();

            if(statusCode == 429 && res.Headers.TryGetValues("X-Contentful-RateLimit-Reset", out headers))
            {
                var rateLimitException = new ContentfulRateLimitException(message)
                {
                    RequestId = jsonError.SelectToken("$.requestId")?.ToString(),
                    ErrorDetails = errorDetails,
                    SystemProperties = sys,
                    SecondsUntilNextRequest = headers.FirstOrDefault() == null ? 0 : int.Parse(headers.FirstOrDefault())
                };

                throw rateLimitException;
            }

            if(statusCode == 504)
            {
                var gatewayTimeoutException = new GatewayTimeoutException()
                {
                    RequestId = jsonError?.SelectToken("$.requestId")?.ToString(),
                    ErrorDetails = errorDetails,
                    SystemProperties = sys
                };

                throw gatewayTimeoutException;
            }

            var ex = new ContentfulException(statusCode, message)
            {
                RequestId = jsonError.SelectToken("$.requestId")?.ToString(),
                ErrorDetails = errorDetails,
                SystemProperties = sys
            };
            throw ex;
        }

        private string GetGenericErrorMessageForStatusCode(int statusCode, string id)
        {
            if(statusCode == 400)
            {
                if (id == "BadRequestError")
                {
                    return "The request was malformed or missing a required parameter.";
                }

                return "The request contained invalid or unknown query parameters.";
            }

            if(statusCode == 401)
            {
                return "The authorization token was invalid.";
            }

            if(statusCode == 403)
            {
                return "The specified token does not have access to the requested resource.";
            }

            if(statusCode == 404)
            {
                return "The requested resource or endpoint could not be found.";
            }

            if (statusCode == 409)
            {
                return "Version mismatch error. The version you specified was incorrect. This may be due to someone else editing the content.";
            }

            if (statusCode == 422)
            {
                if(id == "InvalidEntryError")
                {
                    return "The entered value was invalid.";
                }

                return "Validation failed. The request references an invalid field.";
            }

            if(statusCode == 429)
            {
                return "Rate limit exceeded. Too many requests per second.";
            }

            if(statusCode == 500)
            {
                return "Internal server error.";
            }

            if(statusCode == 502)
            {
                return "The requested space is hibernated.";
            }

            if(statusCode == 504)
            {
                return "Gateway timeout.";
            }

            return "An error occurred.";
        }

        /// <summary>
        /// Sends an Http request.
        /// </summary>
        /// <param name="url">The url to send to.</param>
        /// <param name="method">The HTTP method to use.</param>
        /// <param name="authToken">The authorization token.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="content">The HTTP content.</param>
        /// <param name="version">The version of the content.</param>
        /// <param name="contentTypeId">The contenttype id header.</param>
        /// <param name="organisationId">The organisation it header.</param>
        /// <param name="additionalHeaders">Any additional headers to send with the request.</param>
        /// <returns></returns>
        protected async Task<HttpResponseMessage> SendHttpRequest(string url, HttpMethod method, string authToken, CancellationToken cancellationToken, HttpContent content = null, 
            int? version = null, string contentTypeId = null, string organisationId = null, List<KeyValuePair<string, IEnumerable<string>>> additionalHeaders = null)
        {
            var httpRequestMessage = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = method
            };
            httpRequestMessage.Headers.Add("Authorization", $"Bearer {authToken}");
            
            httpRequestMessage.Headers.Add("X-Contentful-User-Agent", $"{Application}/{Version}; platform {Platform}; os {Os};");

            AddVersionHeader(version, httpRequestMessage);

            if (!string.IsNullOrEmpty(contentTypeId))
            {
                httpRequestMessage.Headers.Add("X-Contentful-Content-Type", contentTypeId);
            }

            if (!string.IsNullOrEmpty(organisationId))
            {
                httpRequestMessage.Headers.Add("X-Contentful-Organization", organisationId);
            }

            if(additionalHeaders != null)
            {
                foreach(var header in additionalHeaders)
                {
                    httpRequestMessage.Headers.Add(header.Key, header.Value);
                }
            }

            httpRequestMessage.Content = content;

            return await SendHttpRequest(httpRequestMessage, cancellationToken).ConfigureAwait(false); ;
        }

        private async Task<HttpResponseMessage> SendHttpRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

            response = await EnsureSuccessfulResult(response);

            return response;
        }

        /// <summary>
        /// Adds a Contentful version header to the request.
        /// </summary>
        /// <param name="version">The version to add.</param>
        /// <param name="message">The message to add the header to.</param>
        protected void AddVersionHeader(int? version, HttpRequestMessage message)
        {
            if (message.Headers.Contains("X-Contentful-Version"))
            {
                message.Headers.Remove("X-Contentful-Version");
            }
            if (version.HasValue)
            {
                message.Headers.Add("X-Contentful-Version", version.ToString());
            }
        }

        /// <summary>
        /// Ensures an HttpResponse is successful.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        protected async Task<HttpResponseMessage> EnsureSuccessfulResult(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                if((int)response.StatusCode == 429 && _options.MaxNumberOfRateLimitRetries > 0)
                {
                    //Limit retries to 10 regardless of config
                    for (var i = 0; i < _options.MaxNumberOfRateLimitRetries && i < 10; i++)
                    {
                        try
                        {
                            await CreateExceptionForFailedRequest(response).ConfigureAwait(false); ;
                        }
                        catch (ContentfulRateLimitException ex)
                        {
                            await Task.Delay(ex.SecondsUntilNextRequest * 1000).ConfigureAwait(false);
                        }
                       
                        var clonedMessage = await CloneHttpRequest(response.RequestMessage);

                        response = await _httpClient.SendAsync(clonedMessage).ConfigureAwait(false);

                        if (response.IsSuccessStatusCode)
                        {
                            return response;
                        }
                    }
                }

                await CreateExceptionForFailedRequest(response);
            }

            return response;
        }

        private async Task<HttpRequestMessage> CloneHttpRequest(HttpRequestMessage message)
        {
            var clone = new HttpRequestMessage(message.Method, message.RequestUri);

            // Copy the request's content (via a MemoryStream) into the cloned object
            var ms = new MemoryStream();
            if (message.Content != null)
            {
                await message.Content.CopyToAsync(ms).ConfigureAwait(false);
                ms.Position = 0;
                clone.Content = new StreamContent(ms);

                if (message.Content.Headers != null)
                    foreach (var h in message.Content.Headers)
                        clone.Content.Headers.Add(h.Key, h.Value);
            }

            foreach (var header in message.Headers)
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

            return clone;
        }
    }
}
