using Contentful.Core.Configuration;
using Contentful.Core.Errors;
using Contentful.Core.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Contentful.Core
{
    /// <summary>
    /// Base class for Contentful clients.
    /// </summary>
    public abstract class ContentfulClientBase
    {
        protected HttpClient _httpClient;
        protected ContentfulOptions _options;

        protected async Task CreateExceptionForFailedRequestAsync(HttpResponseMessage res)
        {
            var jsonError = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var sys = jsonError.SelectToken("$.sys").ToObject<SystemProperties>();
            var errorDetails = jsonError.SelectToken("$.details")?.ToObject<ErrorDetails>();

            var message = jsonError.SelectToken("$.message")?.ToString();
            var statusCode = (int)res.StatusCode;

            if (string.IsNullOrEmpty(message))
            {
                message = GetGenericErrorMessageForStatusCode(statusCode, sys.Id);
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

            return "An error occurred.";
        }

        protected async Task<HttpResponseMessage> SendHttpRequestAsync(string url, HttpMethod method, string authToken, HttpContent content = null)
        {
            var httpRequestMessage = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = method
            };
            httpRequestMessage.Headers.Add("Authorization", $"Bearer {authToken}");
            httpRequestMessage.Headers.Add("User-Agent", "Contentful-.NET-SDK");

            httpRequestMessage.Content = content;

            return await SendHttpRequestAsync(httpRequestMessage).ConfigureAwait(false); ;
        }

        private async Task<HttpResponseMessage> SendHttpRequestAsync(HttpRequestMessage request)
        {
            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

            response = await EnsureSuccessfulResultAsync(response);

            return response;
        }

        protected void AddVersionHeader(int? version)
        {
            if (_httpClient.DefaultRequestHeaders.Contains("X-Contentful-Version"))
            {
                _httpClient.DefaultRequestHeaders.Remove("X-Contentful-Version");
            }
            if (version.HasValue)
            {
                _httpClient.DefaultRequestHeaders.Add("X-Contentful-Version", version.ToString());
            }
        }

        protected void RemoveVersionHeader()
        {
            _httpClient.DefaultRequestHeaders.Remove("X-Contentful-Version");
        }

        protected async Task<HttpResponseMessage> EnsureSuccessfulResultAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                if((int)response.StatusCode == 429 && _options.MaxNumberOfRateLimitRetries > 0)
                {
                    //Limit retries to 10 regardless of config
                    for (int i = 0; i < _options.MaxNumberOfRateLimitRetries && i < 10; i++)
                    {
                        try
                        {
                            await CreateExceptionForFailedRequestAsync(response).ConfigureAwait(false); ;
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

                await CreateExceptionForFailedRequestAsync(response);
            }

            return response;
        }

        private async Task<HttpRequestMessage> CloneHttpRequest(HttpRequestMessage message)
        {
            HttpRequestMessage clone = new HttpRequestMessage(message.Method, message.RequestUri);

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

            foreach (KeyValuePair<string, IEnumerable<string>> header in message.Headers)
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

            return clone;
        }
    }
}
