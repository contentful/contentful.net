using Contentful.Core.Configuration;
using Contentful.Core.Errors;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Contentful.Core
{
    public class ContentfulManagementClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://api.contentful.com/spaces/";
        private readonly ContentfulOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentfulManagementClient"/> class. 
        /// The main class for interaction with the contentful deliver and preview APIs.
        /// </summary>
        /// <param name="httpClient">The HttpClient of your application.</param>
        /// <param name="options">The options object used to retrieve the <see cref="ContentfulOptions"/> for this client.</param>
        /// <exception cref="ArgumentException">The <param name="options">options</param> parameter was null or empty</exception>
        public ContentfulManagementClient(HttpClient httpClient, IOptions<ContentfulOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;

            if (options == null)
            {
                throw new ArgumentException("The ContentfulOptions cannot be null.", nameof(options));
            }

            if (_httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                _httpClient.DefaultRequestHeaders.Remove("Authorization");
            }
           
            if (!_httpClient.DefaultRequestHeaders.Contains("User-Agent"))
            {
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "Contentful-.NET-SDK");
            }

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_options.ManagementApiKey}");

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentfulManagementClient"/> class.
        /// </summary>
        /// <param name="httpClient">The HttpClient of your application.</param>
        /// <param name="options">The <see cref="ContentfulOptions"/> used for this client.</param>
        public ContentfulManagementClient(HttpClient httpClient, ContentfulOptions options):
            this(httpClient, new OptionsWrapper<ContentfulOptions>(options))
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentfulManagementClient"/> class.
        /// </summary>
        /// <param name="httpClient">The HttpClient of your application.</param>
        /// <param name="managementApiKey">The management API key used when communicating with the Contentful API</param>
        /// <param name="spaceId">The ID of the space to fetch content from.</param>
        /// If this is set to true the preview API key needs to be used for <paramref name="deliveryApiKey"/>
        ///  </param>
        public ContentfulManagementClient(HttpClient httpClient, string deliveryApiKey, string spaceId):
            this(httpClient, new OptionsWrapper<ContentfulOptions>(new ContentfulOptions()
            {
                DeliveryApiKey = deliveryApiKey,
                SpaceId = spaceId
            }))
        {

        }

        /// <summary>
        /// Creates a new space in Contentful.
        /// </summary>
        /// <param name="name">The name of the space to create.</param>
        /// <param name="defaultLocale">The default locale for this space.</param>
        /// <param name="organisation">The organisation to create a space for. Not required if the account belongs to only one organisation.</param>
        /// <returns>The created <see cref="Space"/></returns>
        public async Task<Space> CreateSpaceAsync(string name, string defaultLocale, string organisation = null)
        {
            if (!string.IsNullOrEmpty(organisation))
            {
                _httpClient.DefaultRequestHeaders.Add("X-Contentful-Organization", organisation);
            }

            var res = await _httpClient.PostAsync(_baseUrl, ConvertObjectToJsonStringContent(new { name = name, defaultLocale = defaultLocale }));

            _httpClient.DefaultRequestHeaders.Remove("X-Contentful-Organization");

            if (!res.IsSuccessStatusCode)
            {
                await CreateExceptionForFailedRequestAsync(res);
            }

            var json = JObject.Parse(await res.Content.ReadAsStringAsync());

            return json.ToObject<Space>();
        }

        /// <summary>
        /// Updates the name of a space in Contentful.
        /// </summary>
        /// <param name="space">The space to update, needs to contain at minimum name, Id and version.</param>
        /// <param name="organisation">The organisation to update a space for. Not required if the account belongs to only one organisation.</param>
        /// <returns>The updated <see cref="Space"/></returns>
        public async Task<Space> UpdateSpaceNameAsync(Space space, string organisation = null)
        {
            return await UpdateSpaceNameAsync(space.SystemProperties.Id, space.Name, space.SystemProperties.Version ?? 1, organisation);
        }

        /// <summary>
        /// Updates a space in Contentful.
        /// </summary>
        /// <param name="id">The id of the space to update.</param>
        /// <param name="name">The name to update to.</param>
        /// <param name="version">The version of the space that will be updated.</param>
        /// <param name="organisation">The organisation to update a space for. Not required if the account belongs to only one organisation.</param>
        /// <returns>The updated <see cref="Space"/></returns>
        public async Task<Space> UpdateSpaceNameAsync(string id, string name, int version, string organisation = null)
        {
            if (!string.IsNullOrEmpty(organisation))
            {
                _httpClient.DefaultRequestHeaders.Add("X-Contentful-Organization", organisation);
            }

            AddVersionHeader(version);

            var res = await _httpClient.PutAsync($"{_baseUrl}{id}", ConvertObjectToJsonStringContent(new { name = name }));

            _httpClient.DefaultRequestHeaders.Remove("X-Contentful-Organization");

            RemoveVersionHeader();

            if (!res.IsSuccessStatusCode)
            {
                await CreateExceptionForFailedRequestAsync(res);
            }

            var json = JObject.Parse(await res.Content.ReadAsStringAsync());

            return json.ToObject<Space>();
        }

        /// <summary>
        /// Gets a space in Contentful.
        /// </summary>
        /// <param name="id">The id of the space to get.</param>
        /// <returns>The <see cref="Space" /></returns>
        public async Task<Space> GetSpaceAsync(string id)
        {
            var res = await _httpClient.GetAsync($"{_baseUrl}{id}");

            if (!res.IsSuccessStatusCode)
            {
                await CreateExceptionForFailedRequestAsync(res);
            }

            var json = JObject.Parse(await res.Content.ReadAsStringAsync());

            return json.ToObject<Space>();
        }

        /// <summary>
        /// Gets all spaces in Contentful.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Space>> GetSpacesAsync()
        {
            var res = await _httpClient.GetAsync(_baseUrl);

            if (!res.IsSuccessStatusCode)
            {
                await CreateExceptionForFailedRequestAsync(res);
            }

            var json = JObject.Parse(await res.Content.ReadAsStringAsync());

            return json.SelectTokens("$..items[*]").Select(t => t.ToObject<Space>());
        }

        /// <summary>
        /// Deletes a space in Contentful.
        /// </summary>
        /// <param name="id">The id of the space to delete.</param>
        /// <returns></returns>
        public async Task DeleteSpaceAsync(string id)
        {
            var res = await _httpClient.DeleteAsync($"{_baseUrl}{id}");

            if (!res.IsSuccessStatusCode)
            {
                await CreateExceptionForFailedRequestAsync(res);
            }
        }

        /// <summary>
        /// Get all content types of a space.
        /// </summary>
        /// <param name="spaceId">The id of the space to get the content types of. Will default to the one set when creating the client.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ContentType"/>.</returns>
        public async Task<IEnumerable<ContentType>> GetContentTypesAsync(string spaceId = null)
        {
            var res = await _httpClient.GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/content_types");

            if (!res.IsSuccessStatusCode)
            {
                await CreateExceptionForFailedRequestAsync(res);
            }

            var json = JObject.Parse(await res.Content.ReadAsStringAsync());

            return json.SelectTokens("$..items[*]").Select(t => t.ToObject<ContentType>());
        }

        /// <summary>
        /// Creates or updates a ContentType. Updates if a content type with the same id already exists.
        /// </summary>
        /// <param name="contentType">The <see cref="ContentType"/> to create or update. **Remember to set the id property.**</param>
        /// <param name="spaceId">The id of the space to create the content type in. Will default to the one set when creating the client.</param>
        /// <param name="version">The last version known of the content type. Must be set for existing content types. Should be null if one is created.</param>
        /// <returns>The created or updated <see cref="ContentType"/>.</returns>
        /// <exception cref="ArgumentException">Thrown if the id of the content type is not set.</exception>
        public async Task<ContentType> CreateOrUpdateContentTypeAsync(ContentType contentType, string spaceId = null, int? version = null)
        {
            if(contentType.SystemProperties?.Id == null)
            {
                throw new ArgumentException("The id of the content type must be set.", nameof(contentType));
            }

            AddVersionHeader(version);

            var res = await _httpClient.PutAsync(
                $"{_baseUrl}{spaceId ?? _options.SpaceId}/content_types/{contentType.SystemProperties.Id}",
                ConvertObjectToJsonStringContent(new { name = contentType.Name, fields = contentType.Fields }));

            RemoveVersionHeader();

            if (!res.IsSuccessStatusCode)
            {
                await CreateExceptionForFailedRequestAsync(res);
            }

            var json = JObject.Parse(await res.Content.ReadAsStringAsync());

            return json.ToObject<ContentType>();
        }

        /// <summary>
        /// Gets a <see cref="ContentType"/> by the specified ID.
        /// </summary>
        /// <param name="contentTypeId">The ID of the content type.</param>
        /// <param name="spaceId">The id of the space to get the content type from. Will default to the one set when creating the client.</param>
        /// <returns>The response from the API serialized into a <see cref="ContentType"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The <param name="contentTypeId">contentTypeId</param> parameter was null or empty</exception>
        public async Task<ContentType> GetContentTypeAsync(string contentTypeId, string spaceId = null)
        {
            if (string.IsNullOrEmpty(contentTypeId))
            {
                throw new ArgumentException(nameof(contentTypeId));
            }

            var res = await _httpClient.GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/content_types/{contentTypeId}");

            if (!res.IsSuccessStatusCode)
            {
                await CreateExceptionForFailedRequestAsync(res);
            }

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync());
            var contentType = jsonObject.ToObject<ContentType>();

            return contentType;
        }

        /// <summary>
        /// Deletes a <see cref="ContentType"/> by the specified ID.
        /// </summary>
        /// <param name="contentTypeId">The ID of the content type.</param>
        /// <param name="spaceId">The id of the space to delete the content type in. Will default to the one set when creating the client.</param>
        /// <returns>The response from the API serialized into a <see cref="ContentType"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The <param name="contentTypeId">contentTypeId</param> parameter was null or empty</exception>
        public async Task DeleteContentTypeAsync(string contentTypeId, string spaceId = null)
        {
            if (string.IsNullOrEmpty(contentTypeId))
            {
                throw new ArgumentException(nameof(contentTypeId));
            }

            var res = await _httpClient.DeleteAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/content_types/{contentTypeId}");

            if (!res.IsSuccessStatusCode)
            {
                await CreateExceptionForFailedRequestAsync(res);
            }

        }

        /// <summary>
        /// Activates a <see cref="ContentType"/> by the specified ID.
        /// </summary>
        /// <param name="contentTypeId">The ID of the content type.</param>
        /// <param name="spaceId">The id of the space to activate the content type in. Will default to the one set when creating the client.</param>
        /// <returns>The response from the API serialized into a <see cref="ContentType"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The <param name="contentTypeId">contentTypeId</param> parameter was null or empty</exception>
        public async Task<ContentType> ActivateContentTypeAsync(string contentTypeId, string spaceId = null)
        {
            if (string.IsNullOrEmpty(contentTypeId))
            {
                throw new ArgumentException(nameof(contentTypeId));
            }

            var res = await _httpClient.PutAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/content_types/{contentTypeId}/published", null);

            if (!res.IsSuccessStatusCode)
            {
                await CreateExceptionForFailedRequestAsync(res);
            }

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync());
            var contentType = jsonObject.ToObject<ContentType>();

            return contentType;
        }

        /// <summary>
        /// Deactivates a <see cref="ContentType"/> by the specified ID.
        /// </summary>
        /// <param name="contentTypeId">The ID of the content type.</param>
        /// <param name="spaceId">The id of the space to deactivate the content type in. Will default to the one set when creating the client.</param>
        /// <returns>The response from the API serialized into a <see cref="ContentType"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The <param name="contentTypeId">contentTypeId</param> parameter was null or empty</exception>
        public async Task DeactivateContentTypeAsync(string contentTypeId, string spaceId = null)
        {
            if (string.IsNullOrEmpty(contentTypeId))
            {
                throw new ArgumentException(nameof(contentTypeId));
            }

            var res = await _httpClient.DeleteAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/content_types/{contentTypeId}/published");

            if (!res.IsSuccessStatusCode)
            {
                await CreateExceptionForFailedRequestAsync(res);
            }
        }

        /// <summary>
        /// Get all activated content types of a space.
        /// </summary>
        /// <param name="spaceId">The id of the space to get the activated content types of. Will default to the one set when creating the client.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ContentType"/>.</returns>
        public async Task<IEnumerable<ContentType>> GetActivatedContentTypesAsync(string spaceId = null)
        {
            var res = await _httpClient.GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/public/content_types");

            if (!res.IsSuccessStatusCode)
            {
                await CreateExceptionForFailedRequestAsync(res);
            }

            var json = JObject.Parse(await res.Content.ReadAsStringAsync());

            return json.SelectTokens("$..items[*]").Select(t => t.ToObject<ContentType>());
        }

        /// <summary>
        /// Gets a <see cref="EditorInterface"/> for a specific <see cref="ContentType"/>.
        /// </summary>
        /// <param name="contentTypeId">The ID of the content type.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <returns>The response from the API serialized into a <see cref="EditorInterface"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The <param name="contentTypeId">contentTypeId</param> parameter was null or empty</exception>
        public async Task<EditorInterface> GetEditorInterfaceAsync(string contentTypeId, string spaceId = null)
        {
            if (string.IsNullOrEmpty(contentTypeId))
            {
                throw new ArgumentException(nameof(contentTypeId));
            }

            var res = await _httpClient.GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/content_types/{contentTypeId}/editor_interface");

            if (!res.IsSuccessStatusCode)
            {
                await CreateExceptionForFailedRequestAsync(res);
            }

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync());
            var editorInterface = jsonObject.ToObject<EditorInterface>();

            return editorInterface;
        }

        /// <summary>
        /// Updates a <see cref="EditorInterface"/> for a specific <see cref="ContentType"/>.
        /// </summary>
        /// <param name="editorInterface">The editor interface to update.</param>
        /// <param name="contentTypeId">The ID of the content type.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <returns>The response from the API serialized into a <see cref="EditorInterface"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The <param name="contentTypeId">contentTypeId</param> parameter was null or empty</exception>
        public async Task<EditorInterface> UpdateEditorInterfaceAsync(EditorInterface editorInterface, string contentTypeId, string spaceId = null, int? version = null)
        {
            if (string.IsNullOrEmpty(contentTypeId))
            {
                throw new ArgumentException(nameof(contentTypeId));
            }

            AddVersionHeader(version);

            var res = await _httpClient.PutAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/content_types/{contentTypeId}/editor_interface", 
                ConvertObjectToJsonStringContent(new { controls = editorInterface.Controls }));

            RemoveVersionHeader();

            if (!res.IsSuccessStatusCode)
            {
                await CreateExceptionForFailedRequestAsync(res);
            }

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync());
            var updatedEditorInterface = jsonObject.ToObject<EditorInterface>();

            return updatedEditorInterface;
        }

        private void AddVersionHeader(int? version)
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

        private void RemoveVersionHeader()
        {
            if (_httpClient.DefaultRequestHeaders.Contains("X-Contentful-Version"))
            {
                _httpClient.DefaultRequestHeaders.Remove("X-Contentful-Version");
            }
        }

        private StringContent ConvertObjectToJsonStringContent(object ob)
        {
            return new StringContent(JsonConvert.SerializeObject(ob), Encoding.UTF8, "application/vnd.contentful.management.v1+json");
        }

        private async Task CreateExceptionForFailedRequestAsync(HttpResponseMessage res)
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
    }
}
