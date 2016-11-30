using Contentful.Core.Configuration;
using Contentful.Core.Errors;
using Contentful.Core.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Contentful.Core
{
    public abstract class ContentfulClientBase
    {
        protected HttpClient _httpClient;
        protected ContentfulOptions _options;

        protected async Task CreateExceptionForFailedRequestAsync(HttpResponseMessage res)
        {
            var jsonError = JObject.Parse(await res.Content.ReadAsStringAsync());
            var sys = jsonError.SelectToken("$.sys").ToObject<SystemProperties>();
            var errorDetails = jsonError.SelectToken("$.details")?.ToObject<ErrorDetails>();
            var ex = new ContentfulException((int)res.StatusCode, jsonError.SelectToken("$.message").ToString())
            {
                RequestId = jsonError.SelectToken("$.requestId").ToString(),
                ErrorDetails = errorDetails,
                SystemProperties = sys
            };
            throw ex;
        }

        protected async Task<HttpResponseMessage> SendHttpRequestAsync(string url, HttpMethod method, string authToken, HttpContent content = null)
        {
            var httpRequestMessage = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = method
            };
            httpRequestMessage.Headers.Add("Authorization", $"Bearer {authToken}");
            httpRequestMessage.Content = content;

            return await _httpClient.SendAsync(httpRequestMessage);
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

        protected async Task EnsureSuccessfulResultAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                await CreateExceptionForFailedRequestAsync(response);
            }
        }
    }
}
