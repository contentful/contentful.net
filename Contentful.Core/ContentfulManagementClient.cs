using Contentful.Core.Configuration;
using Contentful.Core.Errors;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contentful.Core.Search;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Contentful.Core
{
    /// <summary>
    /// Encapsulates methods to interact with the Contentful Management API.
    /// </summary>
    public class ContentfulManagementClient : ContentfulClientBase, IContentfulManagementClient
    {
        private readonly string _directApiUrl = "https://api.contentful.com/";
        private readonly string _baseUrl = "https://api.contentful.com/spaces/";
        private readonly string _baseUploadUrl = "https://upload.contentful.com/spaces/";

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentfulManagementClient"/> class. 
        /// The main class for interaction with the contentful deliver and preview APIs.
        /// </summary>
        /// <param name="httpClient">The HttpClient of your application.</param>
        /// <param name="options">The options object used to retrieve the <see cref="ContentfulOptions"/> for this client.</param>
        /// <exception cref="ArgumentException">The <see name="options">options</see> parameter was null or empty</exception>
        public ContentfulManagementClient(HttpClient httpClient, IOptions<ContentfulOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;

            if (options == null)
            {
                throw new ArgumentException("The ContentfulOptions cannot be null.", nameof(options));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentfulManagementClient"/> class.
        /// </summary>
        /// <param name="httpClient">The HttpClient of your application.</param>
        /// <param name="options">The <see cref="ContentfulOptions"/> used for this client.</param>
        public ContentfulManagementClient(HttpClient httpClient, ContentfulOptions options) :
            this(httpClient, new OptionsWrapper<ContentfulOptions>(options))
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentfulManagementClient"/> class.
        /// </summary>
        /// <param name="httpClient">The HttpClient of your application.</param>
        /// <param name="managementApiKey">The management API key used when communicating with the Contentful API</param>
        /// <param name="spaceId">The id of the space to fetch content from.</param>
        public ContentfulManagementClient(HttpClient httpClient, string managementApiKey, string spaceId) :
            this(httpClient, new OptionsWrapper<ContentfulOptions>(new ContentfulOptions()
            {
                ManagementApiKey = managementApiKey,
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
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Space"/></returns>
        public async Task<Space> CreateSpaceAsync(string name, string defaultLocale, string organisation = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!string.IsNullOrEmpty(organisation))
            {
                _httpClient.DefaultRequestHeaders.Add("X-Contentful-Organization", organisation);
            }

            var res = await PostAsync(_baseUrl, ConvertObjectToJsonStringContent(new { name = name, defaultLocale = defaultLocale }), cancellationToken).ConfigureAwait(false);

            _httpClient.DefaultRequestHeaders.Remove("X-Contentful-Organization");

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var json = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return json.ToObject<Space>();
        }

        /// <summary>
        /// Updates the name of a space in Contentful.
        /// </summary>
        /// <param name="space">The space to update, needs to contain at minimum name, Id and version.</param>
        /// <param name="organisation">The organisation to update a space for. Not required if the account belongs to only one organisation.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The updated <see cref="Space"/></returns>
        public async Task<Space> UpdateSpaceNameAsync(Space space, string organisation = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await UpdateSpaceNameAsync(space.SystemProperties.Id, space.Name, space.SystemProperties.Version ?? 1, organisation, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Updates a space in Contentful.
        /// </summary>
        /// <param name="id">The id of the space to update.</param>
        /// <param name="name">The name to update to.</param>
        /// <param name="version">The version of the space that will be updated.</param>
        /// <param name="organisation">The organisation to update a space for. Not required if the account belongs to only one organisation.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The updated <see cref="Space"/></returns>
        public async Task<Space> UpdateSpaceNameAsync(string id, string name, int version, string organisation = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!string.IsNullOrEmpty(organisation))
            {
                _httpClient.DefaultRequestHeaders.Add("X-Contentful-Organization", organisation);
            }

            AddVersionHeader(version);

            var res = await PutAsync($"{_baseUrl}{id}", ConvertObjectToJsonStringContent(new { name = name }), cancellationToken).ConfigureAwait(false);

            _httpClient.DefaultRequestHeaders.Remove("X-Contentful-Organization");

            RemoveVersionHeader();

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var json = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return json.ToObject<Space>();
        }

        /// <summary>
        /// Gets a space in Contentful.
        /// </summary>
        /// <param name="id">The id of the space to get.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Space" /></returns>
        public async Task<Space> GetSpaceAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var res = await GetAsync($"{_baseUrl}{id}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var json = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return json.ToObject<Space>();
        }

        /// <summary>
        /// Gets all spaces in Contentful.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Space"/>.</returns>
        public async Task<IEnumerable<Space>> GetSpacesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var res = await GetAsync(_baseUrl, cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var json = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return json.SelectTokens("$..items[*]").Select(t => t.ToObject<Space>());
        }

        /// <summary>
        /// Deletes a space in Contentful.
        /// </summary>
        /// <param name="id">The id of the space to delete.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns></returns>
        public async Task DeleteSpaceAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var res = await DeleteAsync($"{_baseUrl}{id}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);
        }

        /// <summary>
        /// Get all content types of a space.
        /// </summary>
        /// <param name="spaceId">The id of the space to get the content types of. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ContentType"/>.</returns>
        public async Task<IEnumerable<ContentType>> GetContentTypesAsync(string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/content_types", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var json = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return json.SelectTokens("$..items[*]").Select(t => t.ToObject<ContentType>());
        }

        /// <summary>
        /// Creates or updates a ContentType. Updates if a content type with the same id already exists.
        /// </summary>
        /// <param name="contentType">The <see cref="ContentType"/> to create or update. **Remember to set the id property.**</param>
        /// <param name="spaceId">The id of the space to create the content type in. Will default to the one set when creating the client.</param>
        /// <param name="version">The last version known of the content type. Must be set for existing content types. Should be null if one is created.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created or updated <see cref="ContentType"/>.</returns>
        /// <exception cref="ArgumentException">Thrown if the id of the content type is not set.</exception>
        public async Task<ContentType> CreateOrUpdateContentTypeAsync(ContentType contentType, string spaceId = null, int? version = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (contentType.SystemProperties?.Id == null)
            {
                throw new ArgumentException("The id of the content type must be set.", nameof(contentType));
            }

            AddVersionHeader(version);

            var res = await PutAsync(
                $"{_baseUrl}{spaceId ?? _options.SpaceId}/content_types/{contentType.SystemProperties.Id}",
                ConvertObjectToJsonStringContent(new { name = contentType.Name, description = contentType.Description, displayField = contentType.DisplayField, fields = contentType.Fields }), cancellationToken).ConfigureAwait(false);

            RemoveVersionHeader();

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var json = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return json.ToObject<ContentType>();
        }

        /// <summary>
        /// Gets a <see cref="ContentType"/> by the specified id.
        /// </summary>
        /// <param name="contentTypeId">The id of the content type.</param>
        /// <param name="spaceId">The id of the space to get the content type from. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into a <see cref="ContentType"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The <see name="contentTypeId">contentTypeId</see> parameter was null or empty</exception>
        public async Task<ContentType> GetContentTypeAsync(string contentTypeId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(contentTypeId))
            {
                throw new ArgumentException(nameof(contentTypeId));
            }

            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/content_types/{contentTypeId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var contentType = jsonObject.ToObject<ContentType>();

            return contentType;
        }

        /// <summary>
        /// Deletes a <see cref="ContentType"/> by the specified id.
        /// </summary>
        /// <param name="contentTypeId">The id of the content type.</param>
        /// <param name="spaceId">The id of the space to delete the content type in. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into a <see cref="ContentType"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The <see name="contentTypeId">contentTypeId</see> parameter was null or empty</exception>
        public async Task DeleteContentTypeAsync(string contentTypeId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(contentTypeId))
            {
                throw new ArgumentException(nameof(contentTypeId));
            }

            var res = await DeleteAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/content_types/{contentTypeId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);
        }

        /// <summary>
        /// Activates a <see cref="ContentType"/> by the specified id.
        /// </summary>
        /// <param name="contentTypeId">The id of the content type.</param>
        /// <param name="version">The last known version of the content type.</param>
        /// <param name="spaceId">The id of the space to activate the content type in. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into a <see cref="ContentType"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The <see name="contentTypeId">contentTypeId</see> parameter was null or empty</exception>
        public async Task<ContentType> ActivateContentTypeAsync(string contentTypeId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(contentTypeId))
            {
                throw new ArgumentException(nameof(contentTypeId));
            }

            AddVersionHeader(version);

            var res = await PutAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/content_types/{contentTypeId}/published", null, cancellationToken).ConfigureAwait(false);

            RemoveVersionHeader();

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var contentType = jsonObject.ToObject<ContentType>();

            return contentType;
        }

        /// <summary>
        /// Deactivates a <see cref="ContentType"/> by the specified id.
        /// </summary>
        /// <param name="contentTypeId">The id of the content type.</param>
        /// <param name="spaceId">The id of the space to deactivate the content type in. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into a <see cref="ContentType"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The <see name="contentTypeId">contentTypeId</see> parameter was null or empty</exception>
        public async Task DeactivateContentTypeAsync(string contentTypeId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(contentTypeId))
            {
                throw new ArgumentException(nameof(contentTypeId));
            }

            var res = await DeleteAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/content_types/{contentTypeId}/published", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);
        }

        /// <summary>
        /// Get all activated content types of a space.
        /// </summary>
        /// <param name="spaceId">The id of the space to get the activated content types of. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ContentType"/>.</returns>
        public async Task<IEnumerable<ContentType>> GetActivatedContentTypesAsync(string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/public/content_types", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var json = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return json.SelectTokens("$..items[*]").Select(t => t.ToObject<ContentType>());
        }

        /// <summary>
        /// Gets a <see cref="Contentful.Core.Models.Management.EditorInterface"/> for a specific <seealso cref="ContentType"/>.
        /// </summary>
        /// <param name="contentTypeId">The id of the content type.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into a <see cref="Contentful.Core.Models.Management.EditorInterface"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The <see name="contentTypeId">contentTypeId</see> parameter was null or empty</exception>
        public async Task<EditorInterface> GetEditorInterfaceAsync(string contentTypeId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(contentTypeId))
            {
                throw new ArgumentException(nameof(contentTypeId));
            }

            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/content_types/{contentTypeId}/editor_interface", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var editorInterface = jsonObject.ToObject<EditorInterface>();

            return editorInterface;
        }

        /// <summary>
        /// Updates a <see cref="Contentful.Core.Models.Management.EditorInterface"/> for a specific <see cref="ContentType"/>.
        /// </summary>
        /// <param name="editorInterface">The editor interface to update.</param>
        /// <param name="contentTypeId">The id of the content type.</param>
        /// <param name="version">The last known version of the content type.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into a <see cref="Contentful.Core.Models.Management.EditorInterface"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The <see name="contentTypeId">contentTypeId</see> parameter was null or empty</exception>
        public async Task<EditorInterface> UpdateEditorInterfaceAsync(EditorInterface editorInterface, string contentTypeId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(contentTypeId))
            {
                throw new ArgumentException(nameof(contentTypeId));
            }

            AddVersionHeader(version);

            var res = await PutAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/content_types/{contentTypeId}/editor_interface",
                ConvertObjectToJsonStringContent(new { controls = editorInterface.Controls }), cancellationToken).ConfigureAwait(false);

            RemoveVersionHeader();

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var updatedEditorInterface = jsonObject.ToObject<EditorInterface>();

            return updatedEditorInterface;
        }

        /// <summary>
        /// Gets all the entries of a space, filtered by an optional <see cref="QueryBuilder{T}"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="IContentfulResource"/> to serialize the response into.</typeparam>
        /// <param name="queryBuilder">The optional <see cref="QueryBuilder{T}"/> to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of items.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<T>> GetEntriesCollectionAsync<T>(QueryBuilder<T> queryBuilder, CancellationToken cancellationToken = default(CancellationToken)) where T : IContentfulResource
        {
            return await GetEntriesCollectionAsync<T>(queryBuilder?.Build(), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all the entries of a space, filtered by an optional querystring. A simpler approach than 
        /// to construct a query manually is to use the <see cref="QueryBuilder{T}"/> class.
        /// </summary>
        /// <typeparam name="T">The <see cref="IContentfulResource"/> to serialize the response into.</typeparam>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of items.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<T>> GetEntriesCollectionAsync<T>(string queryString = null, CancellationToken cancellationToken = default(CancellationToken)) where T : IContentfulResource
        {
            var res = await GetAsync($"{_baseUrl}{_options.SpaceId}/entries{queryString}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var collection = jsonObject.ToObject<ContentfulCollection<T>>();

            return collection;
        }

        /// <summary>
        /// Creates an <see cref="Entry{T}"/>.
        /// </summary>
        /// <param name="entry">The entry to create or update.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="contentTypeId">The id of the <see cref="ContentType"/> of the entry.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Entry{T}"/>.</returns>
        public async Task<Entry<dynamic>> CreateEntryAsync(Entry<dynamic> entry, string contentTypeId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(contentTypeId))
            {
                throw new ArgumentException("The content type id must be set.", nameof(contentTypeId));
            }

            _httpClient.DefaultRequestHeaders.Remove("X-Contentful-Content-Type");
            _httpClient.DefaultRequestHeaders.Add("X-Contentful-Content-Type", contentTypeId);

            var res = await PostAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/entries",
                ConvertObjectToJsonStringContent(new { fields = entry.Fields }), cancellationToken).ConfigureAwait(false);

            _httpClient.DefaultRequestHeaders.Remove("X-Contentful-Content-Type");

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var updatedEntry = jsonObject.ToObject<Entry<dynamic>>();

            return updatedEntry;
        }

        /// <summary>
        /// Creates or updates an <see cref="Entry{T}"/>. Updates if an entry with the same id already exists.
        /// </summary>
        /// <param name="entry">The entry to create or update.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="contentTypeId">The id of the <see cref="ContentType"/> of the entry. Need only be set if you are creating a new entry.</param>
        /// <param name="version">The last known version of the entry. Must be set when updating an entry.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created or updated <see cref="Entry{T}"/>.</returns>
        public async Task<Entry<dynamic>> CreateOrUpdateEntryAsync(Entry<dynamic> entry, string spaceId = null, string contentTypeId = null, int? version = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(entry.SystemProperties?.Id))
            {
                throw new ArgumentException("The id of the entry must be set.");
            }

            if (!string.IsNullOrEmpty(contentTypeId))
            {
                _httpClient.DefaultRequestHeaders.Remove("X-Contentful-Content-Type");
                _httpClient.DefaultRequestHeaders.Add("X-Contentful-Content-Type", contentTypeId);
            }

            AddVersionHeader(version);

            var res = await PutAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/entries/{entry.SystemProperties.Id}",
                ConvertObjectToJsonStringContent(new { fields = entry.Fields }), cancellationToken).ConfigureAwait(false);

            RemoveVersionHeader();
            _httpClient.DefaultRequestHeaders.Remove("X-Contentful-Content-Type");

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var updatedEntry = jsonObject.ToObject<Entry<dynamic>>();

            return updatedEntry;
        }

        /// <summary>
        /// Get a single entry by the specified id.
        /// </summary>
        /// <param name="entryId">The id of the entry.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into <see cref="Entry{dynamic}"/></returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The <see name="entryId">entryId</see> parameter was null or empty.</exception>
        public async Task<Entry<dynamic>> GetEntryAsync(string entryId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(entryId))
            {
                throw new ArgumentException(nameof(entryId));
            }

            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/entries/{entryId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            return JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false)).ToObject<Entry<dynamic>>();
        }

        /// <summary>
        /// Deletes a single entry by the specified id.
        /// </summary>
        /// <param name="entryId">The id of the entry.</param>
        /// <param name="version">The last known version of the entry.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The <see name="entryId">entryId</see> parameter was null or empty.</exception>
        public async Task DeleteEntryAsync(string entryId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(entryId))
            {
                throw new ArgumentException(nameof(entryId));
            }

            AddVersionHeader(version);

            var res = await DeleteAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/entries/{entryId}", cancellationToken).ConfigureAwait(false);

            RemoveVersionHeader();

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);
        }

        /// <summary>
        /// Publishes an entry by the specified id.
        /// </summary>
        /// <param name="entryId">The id of the entry.</param>
        /// <param name="version">The last known version of the entry.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into <see cref="Entry{dynamic}"/></returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The <see name="entryId">entryId</see> parameter was null or empty.</exception>
        public async Task<Entry<dynamic>> PublishEntryAsync(string entryId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(entryId))
            {
                throw new ArgumentException(nameof(entryId));
            }

            AddVersionHeader(version);

            var res = await PutAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/entries/{entryId}/published", null, cancellationToken).ConfigureAwait(false);

            RemoveVersionHeader();

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            return JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false)).ToObject<Entry<dynamic>>();
        }

        /// <summary>
        /// Unpublishes an entry by the specified id.
        /// </summary>
        /// <param name="entryId">The id of the entry.</param>
        /// <param name="version">The last known version of the entry.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into <see cref="Entry{dynamic}"/></returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The <see name="entryId">entryId</see> parameter was null or empty.</exception>
        public async Task<Entry<dynamic>> UnpublishEntryAsync(string entryId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(entryId))
            {
                throw new ArgumentException(nameof(entryId));
            }

            AddVersionHeader(version);

            var res = await DeleteAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/entries/{entryId}/published", cancellationToken).ConfigureAwait(false);

            RemoveVersionHeader();

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            return JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false)).ToObject<Entry<dynamic>>();
        }

        /// <summary>
        /// Archives an entry by the specified id.
        /// </summary>
        /// <param name="entryId">The id of the entry.</param>
        /// <param name="version">The last known version of the entry.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into <see cref="Entry{dynamic}"/></returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The <see name="entryId">entryId</see> parameter was null or empty.</exception>
        public async Task<Entry<dynamic>> ArchiveEntryAsync(string entryId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(entryId))
            {
                throw new ArgumentException(nameof(entryId));
            }

            AddVersionHeader(version);

            var res = await PutAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/entries/{entryId}/archived", null, cancellationToken).ConfigureAwait(false);

            RemoveVersionHeader();

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            return JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false)).ToObject<Entry<dynamic>>();
        }

        /// <summary>
        /// Unarchives an entry by the specified id.
        /// </summary>
        /// <param name="entryId">The id of the entry.</param>
        /// <param name="version">The last known version of the entry.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into <see cref="Entry{dynamic}"/></returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The <see name="entryId">entryId</see> parameter was null or empty.</exception>
        public async Task<Entry<dynamic>> UnarchiveEntryAsync(string entryId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(entryId))
            {
                throw new ArgumentException(nameof(entryId));
            }

            AddVersionHeader(version);

            var res = await DeleteAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/entries/{entryId}/archived", cancellationToken).ConfigureAwait(false);

            RemoveVersionHeader();

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            return JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false)).ToObject<Entry<dynamic>>();
        }

        /// <summary>
        /// Gets all assets in the space.
        /// </summary>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.ManagementAsset"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<ManagementAsset>> GetAssetsCollectionAsync(string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/assets", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var collection = jsonObject.ToObject<ContentfulCollection<ManagementAsset>>();
            var assets = jsonObject.SelectTokens("$..items[*]").Select(c => c.ToObject<ManagementAsset>());
            collection.Items = assets;

            return collection;
        }

        /// <summary>
        /// Gets all published assets in the space.
        /// </summary>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.ManagementAsset"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<ManagementAsset>> GetPublishedAssetsCollectionAsync(string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/public/assets", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var collection = jsonObject.ToObject<ContentfulCollection<ManagementAsset>>();
            var assets = jsonObject.SelectTokens("$..items[*]").Select(c => c.ToObject<ManagementAsset>());
            collection.Items = assets;

            return collection;
        }

        /// <summary>
        /// Gets an asset by the specified id.
        /// </summary>
        /// <param name="assetId">The id of the asset to get.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.ManagementAsset"/>.</returns>
        /// <exception cref="ArgumentException">The <see name="assetId">assetId</see> parameter was null or empty.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ManagementAsset> GetAssetAsync(string assetId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(assetId))
            {
                throw new ArgumentException(nameof(assetId));
            }

            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/assets/{assetId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<ManagementAsset>();
        }

        /// <summary>
        /// Deletes an asset by the specified id.
        /// </summary>
        /// <param name="assetId">The id of the asset to delete.</param>
        /// <param name="version">The last known version of the asset.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <exception cref="ArgumentException">The <see name="assetId">assetId</see> parameter was null or empty.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task DeleteAssetAsync(string assetId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(assetId))
            {
                throw new ArgumentException(nameof(assetId));
            }

            AddVersionHeader(version);

            var res = await DeleteAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/assets/{assetId}", cancellationToken).ConfigureAwait(false);

            RemoveVersionHeader();

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);
        }

        /// <summary>
        /// Publishes an asset by the specified id.
        /// </summary>
        /// <param name="assetId">The id of the asset to publish.</param>
        /// <param name="version">The last known version of the asset.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.ManagementAsset"/> published.</returns>
        /// <exception cref="ArgumentException">The <see name="assetId">assetId</see> parameter was null or empty.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ManagementAsset> PublishAssetAsync(string assetId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(assetId))
            {
                throw new ArgumentException(nameof(assetId));
            }

            AddVersionHeader(version);

            var res = await PutAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/assets/{assetId}/published", null, cancellationToken).ConfigureAwait(false);

            RemoveVersionHeader();

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<ManagementAsset>();
        }

        /// <summary>
        /// Unpublishes an asset by the specified id.
        /// </summary>
        /// <param name="assetId">The id of the asset to unpublish.</param>
        /// <param name="version">The last known version of the asset.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.ManagementAsset"/> unpublished.</returns>
        /// <exception cref="ArgumentException">The <see name="assetId">assetId</see> parameter was null or empty.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ManagementAsset> UnpublishAssetAsync(string assetId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(assetId))
            {
                throw new ArgumentException(nameof(assetId));
            }

            AddVersionHeader(version);

            var res = await DeleteAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/assets/{assetId}/published", cancellationToken).ConfigureAwait(false);

            RemoveVersionHeader();

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<ManagementAsset>();
        }

        /// <summary>
        /// Archives an asset by the specified id.
        /// </summary>
        /// <param name="assetId">The id of the asset to archive.</param>
        /// <param name="version">The last known version of the asset.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.ManagementAsset"/> archived.</returns>
        /// <exception cref="ArgumentException">The <see name="assetId">assetId</see> parameter was null or empty.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ManagementAsset> ArchiveAssetAsync(string assetId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(assetId))
            {
                throw new ArgumentException(nameof(assetId));
            }

            AddVersionHeader(version);

            HttpResponseMessage res = null;

            res = await PutAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/assets/{assetId}/archived", null, cancellationToken).ConfigureAwait(false);

            RemoveVersionHeader();

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<ManagementAsset>();
        }

        /// <summary>
        /// Unarchives an asset by the specified id.
        /// </summary>
        /// <param name="assetId">The id of the asset to unarchive.</param>
        /// <param name="version">The last known version of the asset.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.ManagementAsset"/> unarchived.</returns>
        /// <exception cref="ArgumentException">The <see name="assetId">assetId</see> parameter was null or empty.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ManagementAsset> UnarchiveAssetAsync(string assetId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(assetId))
            {
                throw new ArgumentException(nameof(assetId));
            }

            AddVersionHeader(version);

            var res = await DeleteAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/assets/{assetId}/archived", cancellationToken).ConfigureAwait(false);

            RemoveVersionHeader();

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<ManagementAsset>();
        }

        /// <summary>
        /// Processes an asset by the specified id.
        /// </summary>
        /// <param name="assetId">The id of the asset to process.</param>
        /// <param name="version">The last known version of the asset.</param>
        /// <param name="locale">The locale for which files should be processed.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <exception cref="ArgumentException">The <see name="assetId">assetId</see> parameter was null or empty.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task ProcessAssetAsync(string assetId, int version, string locale, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(assetId))
            {
                throw new ArgumentException(nameof(assetId));
            }

            AddVersionHeader(version);

            HttpResponseMessage res = null;

            res = await PutAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/assets/{assetId}/files/{locale}/process", null, cancellationToken).ConfigureAwait(false);

            RemoveVersionHeader();

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates or updates an <see cref="Contentful.Core.Models.Management.ManagementAsset"/>. Updates if an asset with the same id already exists.
        /// </summary>
        /// <param name="asset">The asset to create or update.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="version">The last known version of the entry. Must be set when updating an asset.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The updated <see cref="Contentful.Core.Models.Management.ManagementAsset"/></returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ManagementAsset> CreateOrUpdateAssetAsync(ManagementAsset asset, string spaceId = null, int? version = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(asset.SystemProperties?.Id))
            {
                throw new ArgumentException("The id of the asset must be set.");
            }

            AddVersionHeader(version);

            var res = await PutAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/assets/{asset.SystemProperties.Id}",
                ConvertObjectToJsonStringContent(new { fields = new { title = asset.Title, description = asset.Description, file = asset.Files } }), cancellationToken).ConfigureAwait(false);

            RemoveVersionHeader();

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var updatedAsset = jsonObject.ToObject<ManagementAsset>();

            return updatedAsset;
        }

        /// <summary>
        /// Gets all locales in a <see cref="Space"/>.
        /// </summary>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{Locale}"/> of locales.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<Locale>> GetLocalesCollectionAsync(string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/locales", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var collection = jsonObject.ToObject<ContentfulCollection<Locale>>();
            var locales = jsonObject.SelectTokens("$..items[*]").Select(c => c.ToObject<Locale>());
            collection.Items = locales;

            return collection;
        }

        /// <summary>
        /// Creates a locale in the specified <see cref="Space"/>.
        /// </summary>
        /// <param name="locale">The <see cref="Contentful.Core.Models.Management.Locale"/> to create.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.Locale"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<Locale> CreateLocaleAsync(Locale locale, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var res = await PostAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/locales",
                ConvertObjectToJsonStringContent(
                    new
                    {
                        code = locale.Code,
                        contentDeliveryApi = locale.ContentDeliveryApi,
                        contentManagementApi = locale.ContentManagementApi,
                        fallbackCode = locale.FallbackCode,
                        name = locale.Name,
                        optional = locale.Optional
                    }), cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<Locale>();
        }

        /// <summary>
        /// Gets a locale in the specified <see cref="Space"/>.
        /// </summary>
        /// <param name="localeId">The id of the locale to get.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The requested <see cref="Contentful.Core.Models.Management.Locale"/>.</returns>
        /// <exception cref="ArgumentException">The <see name="localeId">localeId</see> parameter was null or empty.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<Locale> GetLocaleAsync(string localeId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(localeId))
            {
                throw new ArgumentException("The localeId must be set.");
            }

            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/locales/{localeId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<Locale>();
        }

        /// <summary>
        /// Updates a locale in the specified <see cref="Space"/>.
        /// </summary>
        /// <param name="locale">The <see cref="Contentful.Core.Models.Management.Locale"/> to update.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.Locale"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<Locale> UpdateLocaleAsync(Locale locale, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(locale.SystemProperties?.Id))
            {
                throw new ArgumentException("The id of the Locale must be set.");
            }

            var res = await PutAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/locales/{locale.SystemProperties.Id}", ConvertObjectToJsonStringContent(new
            {
                code = locale.Code,
                contentDeliveryApi = locale.ContentDeliveryApi,
                contentManagementApi = locale.ContentManagementApi,
                fallbackCode = locale.FallbackCode,
                name = locale.Name,
                optional = locale.Optional
            }), cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<Locale>();
        }

        /// <summary>
        /// Deletes a locale by the specified id.
        /// </summary>
        /// <param name="localeId">The id of the locale to delete.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <exception cref="ArgumentException">The <see name="localeId">localeId</see> parameter was null or empty.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task DeleteLocaleAsync(string localeId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(localeId))
            {
                throw new ArgumentException("The localeId must be set.");
            }

            var res = await DeleteAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/locales/{localeId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all webhooks for a <see cref="Space"/>.
        /// </summary>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.WebHook"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<WebHook>> GetWebHooksCollectionAsync(string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/webhook_definitions", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var collection = jsonObject.ToObject<ContentfulCollection<WebHook>>();
            var hooks = jsonObject.SelectTokens("$..items[*]").Select(c => c.ToObject<WebHook>());
            collection.Items = hooks;

            return collection;
        }

        /// <summary>
        /// Creates a webhook in a <see cref="Space"/>.
        /// </summary>
        /// <param name="webhook">The webhook to create.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.WebHook"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<WebHook> CreateWebHookAsync(WebHook webhook, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            //Not allowed to post system properties
            webhook.SystemProperties = null;

            var res = await PostAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/webhook_definitions", ConvertObjectToJsonStringContent(webhook), cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<WebHook>();
        }

        /// <summary>
        /// Creates or updates a webhook in a <see cref="Space"/>.  Updates if a webhook with the same id already exists.
        /// </summary>
        /// <param name="webhook">The webhook to create or update.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.WebHook"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The id of the webhook parameter was null or empty.</exception>
        public async Task<WebHook> CreateOrUpdateWebHookAsync(WebHook webhook, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(webhook?.SystemProperties?.Id))
            {
                throw new ArgumentException("The id of the webhook must be set.");
            }

            var id = webhook.SystemProperties.Id;

            //Not allowed to post system properties
            webhook.SystemProperties = null;

            var res = await PutAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/webhook_definitions/{id}", ConvertObjectToJsonStringContent(webhook), cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<WebHook>();
        }

        /// <summary>
        /// Gets a single webhook from a <see cref="Space"/>.
        /// </summary>
        /// <param name="webhookId">The id of the webhook to get.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.WebHook"/>.</returns>
        /// <exception cref="ArgumentException">The <see name="webhookId">webhookId</see> parameter was null or empty.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<WebHook> GetWebHookAsync(string webhookId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(webhookId))
            {
                throw new ArgumentException("The id of the webhook must be set.", nameof(webhookId));
            }

            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/webhook_definitions/{webhookId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<WebHook>();
        }

        /// <summary>
        /// Deletes a webhook from a <see cref="Space"/>.
        /// </summary>
        /// <param name="webhookId">The id of the webhook to delete.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <exception cref="ArgumentException">The <see name="webhookId">webhookId</see> parameter was null or empty.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task DeleteWebHookAsync(string webhookId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(webhookId))
            {
                throw new ArgumentException("The id of the webhook must be set", nameof(webhookId));
            }

            var res = await DeleteAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/webhook_definitions/{webhookId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all recent call details for a webhook.
        /// </summary>
        /// <param name="webhookId">The id of the webhook to get details for.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.WebHookCallDetails"/>.</returns>
        /// <exception cref="ArgumentException">The <see name="webhookId">webhookId</see> parameter was null or empty.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<WebHookCallDetails>> GetWebHookCallDetailsCollectionAsync(string webhookId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(webhookId))
            {
                throw new ArgumentException("The id of the webhook must be set.", nameof(webhookId));
            }

            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/webhooks/{webhookId}/calls", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var collection = jsonObject.ToObject<ContentfulCollection<WebHookCallDetails>>();
            var hooks = jsonObject.SelectTokens("$..items[*]").Select(c => c.ToObject<WebHookCallDetails>());
            collection.Items = hooks;

            return collection;
        }

        /// <summary>
        /// Gets the details of a specific webhook call.
        /// </summary>
        /// <param name="callId">The id of the call to get details for.</param>
        /// <param name="webhookId">The id of the webhook to get details for.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.WebHookCallDetails"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The <see name="webhookId">webhookId</see> or <see name="callId">callId</see> parameter was null or empty.</exception>
        public async Task<WebHookCallDetails> GetWebHookCallDetailsAsync(string callId, string webhookId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(callId))
            {
                throw new ArgumentException("The id of the webhook call must be set.", nameof(callId));
            }

            if (string.IsNullOrEmpty(webhookId))
            {
                throw new ArgumentException("The id of the webhook must be set.", nameof(webhookId));
            }

            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/webhooks/{webhookId}/calls/{callId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<WebHookCallDetails>();
        }

        /// <summary>
        /// Gets a response containing an overview of the recent webhook calls.
        /// </summary>
        /// <param name="webhookId">The id of the webhook to get health details for.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="Contentful.Core.Models.Management.WebHookHealthResponse"/>.</returns>
        /// <exception cref="ArgumentException">The <see name="webhookId">webhookId</see> parameter was null or empty.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<WebHookHealthResponse> GetWebHookHealthAsync(string webhookId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(webhookId))
            {
                throw new ArgumentException("The id of the webhook must be set.", nameof(webhookId));
            }

            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/webhooks/{webhookId}/health", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var health = new WebHookHealthResponse()
            {
                SystemProperties = jsonObject["sys"]?.ToObject<SystemProperties>(),
                TotalCalls = jsonObject["calls"]["total"].Value<int>(),
                TotalHealthy = jsonObject["calls"]["healthy"].Value<int>()
            };
            return health;
        }

        /// <summary>
        /// Gets a role by the specified id.
        /// </summary>
        /// <param name="roleId">The id of the role.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.Role"/></returns>
        /// <exception cref="ArgumentException">The <see name="roleId">roleId</see> parameter was null or empty.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<Role> GetRoleAsync(string roleId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(roleId))
            {
                throw new ArgumentException("The id of the role must be set", nameof(roleId));
            }

            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/roles/{roleId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<Role>();
        }

        /// <summary>
        /// Gets all <see cref="Contentful.Core.Models.Management.Role">roles</see> of a space.
        /// </summary>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.Role"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<Role>> GetAllRolesAsync(string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/roles", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var collection = jsonObject.ToObject<ContentfulCollection<Role>>();
            var roles = jsonObject.SelectTokens("$..items[*]").Select(c => c.ToObject<Role>());
            collection.Items = roles;

            return collection;
        }

        /// <summary>
        /// Creates a role in a <see cref="Space"/>.
        /// </summary>
        /// <param name="role">The role to create.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.Role"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<Role> CreateRoleAsync(Role role, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            //Not allowed to post system properties
            role.SystemProperties = null;

            var res = await PostAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/roles", ConvertObjectToJsonStringContent(role), cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<Role>();
        }

        /// <summary>
        /// Updates a role in a <see cref="Space"/>.
        /// </summary>
        /// <param name="role">The role to update.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The updated <see cref="Contentful.Core.Models.Management.Role"/>.</returns>
        /// <exception cref="ArgumentException">The id parameter of the role was null or empty.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<Role> UpdateRoleAsync(Role role, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(role?.SystemProperties?.Id))
            {
                throw new ArgumentException("The id of the role must be set.");
            }

            var id = role.SystemProperties.Id;

            //Not allowed to post system properties
            role.SystemProperties = null;

            var res = await PutAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/roles/{id}", ConvertObjectToJsonStringContent(role), cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<Role>();
        }

        /// <summary>
        /// Deletes a role by the specified id.
        /// </summary>
        /// <param name="roleId">The id of the role to delete.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <exception cref="ArgumentException">The <see name="roleId">roleId</see> parameter was null or empty.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task DeleteRoleAsync(string roleId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(roleId))
            {
                throw new ArgumentException("The id of the role must be set", nameof(roleId));
            }

            var res = await DeleteAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/roles/{roleId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all snapsohts for an <see cref="Entry{T}"/>.
        /// </summary>
        /// <param name="entryId">The id of the entry to get snapshots for.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A collection of <see cref="Contentful.Core.Models.Management.Snapshot"/>.</returns>
        public async Task<ContentfulCollection<Snapshot>> GetAllSnapshotsForEntryAsync(string entryId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(entryId))
            {
                throw new ArgumentException("The id of the entry must be set", nameof(entryId));
            }

            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/entries/{entryId}/snapshots", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var collection = jsonObject.ToObject<ContentfulCollection<Snapshot>>();
            var roles = jsonObject.SelectTokens("$..items[*]").Select(c => c.ToObject<Snapshot>());
            collection.Items = roles;

            return collection;
        }

        /// <summary>
        /// Gets a single snapshot for an <see cref="Entry{T}"/>
        /// </summary>
        /// <param name="snapshotId">The id of the snapshot to get.</param>
        /// <param name="entryId">The id of entry the snapshot belongs to.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.Snapshot"/>.</returns>
        public async Task<Snapshot> GetSnapshotForEntryAsync(string snapshotId, string entryId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(snapshotId))
            {
                throw new ArgumentException("The id of the snapshot must be set.", nameof(snapshotId));
            }

            if (string.IsNullOrEmpty(entryId))
            {
                throw new ArgumentException("The id of the entry must be set.", nameof(entryId));
            }

            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/entries/{entryId}/snapshots/{snapshotId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<Snapshot>();
        }

        /// <summary>
        /// Gets a collection of <see cref="Contentful.Core.Models.Management.SpaceMembership"/> for the user.
        /// </summary>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A collection of <see cref="Contentful.Core.Models.Management.SpaceMembership"/>.</returns>
        public async Task<ContentfulCollection<SpaceMembership>> GetSpaceMembershipsAsync(string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/space_memberships", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var collection = jsonObject.ToObject<ContentfulCollection<SpaceMembership>>();
            var memberships = jsonObject.SelectTokens("$..items[*]").Select(c => c.ToObject<SpaceMembership>());
            collection.Items = memberships;

            return collection;
        }

        /// <summary>
        /// Creates a membership in a <see cref="Space"/>.
        /// </summary>
        /// <param name="spaceMembership">The membership to create.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.SpaceMembership"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<SpaceMembership> CreateSpaceMembershipAsync(SpaceMembership spaceMembership, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var res = await PostAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/space_memberships", ConvertObjectToJsonStringContent(spaceMembership), cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<SpaceMembership>();
        }

        /// <summary>
        /// Gets a single <see cref="Contentful.Core.Models.Management.SpaceMembership"/> for a space.
        /// </summary>
        /// <param name="spaceMembershipId">The id of the space membership to get.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.SpaceMembership"/>.</returns>
        /// <exception cref="ArgumentException">The <see name="spaceMembershipId">spaceMembershipId</see> parameter was null or empty.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<SpaceMembership> GetSpaceMembershipAsync(string spaceMembershipId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(spaceMembershipId))
            {
                throw new ArgumentException("The id of the space membership must be set", nameof(spaceMembershipId));
            }

            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/space_memberships/{spaceMembershipId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<SpaceMembership>();
        }

        /// <summary>
        /// Updates a <see cref="Contentful.Core.Models.Management.SpaceMembership"/> for a space.
        /// </summary>
        /// <param name="spaceMembership">The membership to update.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.SpaceMembership"/>.</returns>
        /// <exception cref="ArgumentException">The <see name="spaceMembership">spaceMembership</see> id was null or empty.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<SpaceMembership> UpdateSpaceMembershipAsync(SpaceMembership spaceMembership, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(spaceMembership?.SystemProperties?.Id))
            {
                throw new ArgumentException("The id of the space membership id must be set", nameof(spaceMembership));
            }

            var res = await PutAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/space_memberships/{spaceMembership.SystemProperties.Id}", ConvertObjectToJsonStringContent(spaceMembership), cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<SpaceMembership>();
        }

        /// <summary>
        /// Deletes a <see cref="Contentful.Core.Models.Management.SpaceMembership"/> for a space.
        /// </summary>
        /// <param name="spaceMembershipId">The id of the space membership to delete.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <exception cref="ArgumentException">The <see name="spaceMembershipId">spaceMembershipId</see> parameter was null or empty.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task DeleteSpaceMembershipAsync(string spaceMembershipId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(spaceMembershipId))
            {
                throw new ArgumentException("The id of the space membership must be set", nameof(spaceMembershipId));
            }

            var res = await DeleteAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/space_memberships/{spaceMembershipId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a collection of all <see cref="Contentful.Core.Models.Management.ApiKey"/> in a space.
        /// </summary>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.ApiKey"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<ApiKey>> GetAllApiKeysAsync(string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/api_keys", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var collection = jsonObject.ToObject<ContentfulCollection<ApiKey>>();
            var keys = jsonObject.SelectTokens("$..items[*]").Select(c => c.ToObject<ApiKey>());
            collection.Items = keys;

            return collection;
        }

        /// <summary>
        /// Creates an <see cref="Contentful.Core.Models.Management.ApiKey"/> in a space.
        /// </summary>
        /// <param name="name">The name of the API key to create.</param>
        /// <param name="description">The description of the API key to create.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.ApiKey"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ApiKey> CreateApiKeyAsync(string name, string description, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("The name of the api key must be set.", nameof(name));
            }

            var res = await PostAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/api_keys", ConvertObjectToJsonStringContent(new { name = name, description = description }), cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<ApiKey>();
        }

        /// <summary>
        /// Gets a collection of all <see cref="Contentful.Core.Models.Management.User"/> in a space.
        /// </summary>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.ApiKey"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<User>> GetAllUsersAsync(string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/users", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var collection = jsonObject.ToObject<ContentfulCollection<User>>();
            var keys = jsonObject.SelectTokens("$..items[*]").Select(c => c.ToObject<User>());
            collection.Items = keys;

            return collection;
        }

        /// <summary>
        /// Gets a single <see cref="Contentful.Core.Models.Management.User"/> for a space.
        /// </summary>
        /// <param name="userId">The id of the user to get.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.User"/>.</returns>
        /// <exception cref="ArgumentException">The <see name="spaceMembershipId">spaceMembershipId</see> parameter was null or empty.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<User> GetUserAsync(string userId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("The id of the user must be set", nameof(userId));
            }

            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/users/{userId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<User>();
        }

        /// <summary>
        /// Gets an upload <see cref="SystemProperties"/> by the specified id.
        /// </summary>
        /// <param name="uploadId">The id of the uploaded file.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="SystemProperties"/> with metadata of the upload.</returns>
        public async Task<UploadReference> GetUploadAsync(string uploadId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var res = await GetAsync($"{_baseUploadUrl}{spaceId ?? _options.SpaceId}/uploads/{uploadId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<UploadReference>();
        }

        /// <summary>
        /// Uploads the specified bytes to Contentful.
        /// </summary>
        /// <param name="bytes">The bytes to upload.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="SystemProperties"/> with an id of the created upload.</returns>
        public async Task<UploadReference> UploadFileAsync(byte[] bytes, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var byteArrayContent = new ByteArrayContent(bytes);
            byteArrayContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

            var res = await PostAsync($"{_baseUploadUrl}{spaceId ?? _options.SpaceId}/uploads", byteArrayContent, cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<UploadReference>();
        }

        /// <summary>
        /// Gets an upload <see cref="SystemProperties"/> by the specified id.
        /// </summary>
        /// <param name="uploadId">The id of the uploaded file.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="SystemProperties"/> with metadata of the upload.</returns>
        public async Task DeleteUploadAsync(string uploadId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var res = await DeleteAsync($"{_baseUploadUrl}{spaceId ?? _options.SpaceId}/uploads/{uploadId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);
        }

        /// <summary>
        /// Uploads an array of bytes and creates an asset in Contentful as well as processing that asset.
        /// </summary>
        /// <param name="asset">The asset to create</param>
        /// <param name="bytes">The bytes to upload.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.ManagementAsset"/>.</returns>
        public async Task<ManagementAsset> UploadFileAndCreateAssetAsync(ManagementAsset asset, byte[] bytes, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var upload = await UploadFileAsync(bytes, spaceId, cancellationToken);
            upload.SystemProperties.CreatedAt = null;
            upload.SystemProperties.CreatedBy = null;
            upload.SystemProperties.Space = null;
            upload.SystemProperties.LinkType = "Upload";
            foreach (var file in asset.Files)
            {
                file.Value.UploadReference = upload;
            }

            var createdAsset = await CreateOrUpdateAssetAsync(asset);

            foreach (var file in createdAsset.Files) {

                await ProcessAssetAsync(createdAsset.SystemProperties.Id, createdAsset.SystemProperties.Version ?? 1, file.Key);
            }

            return createdAsset;
        }

        /// <summary>
        /// Gets a collection of all <see cref="Contentful.Core.Models.Management.UiExtension"/> for a space.
        /// </summary>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.UiExtension"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<UiExtension>> GetAllExtensionsAsync(string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/extensions", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var collection = jsonObject.ToObject<ContentfulCollection<UiExtension>>();
            var keys = jsonObject.SelectTokens("$..items[*]").Select(c => c.ToObject<UiExtension>());
            collection.Items = keys;

            return collection;
        }

        /// <summary>
        /// Creates a UiExtension in a <see cref="Space"/>.
        /// </summary>
        /// <param name="extension">The UI extension to create.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.UiExtension"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<UiExtension> CreateExtensionAsync(UiExtension extension, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var res = await PostAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/extensions",
                ConvertObjectToJsonStringContent(new
                {
                    extension = new
                    {
                        src = extension.Src,
                        name = extension.Name,
                        fieldTypes = extension.FieldTypes?.Select(c => new { type = c }),
                        srcDoc = extension.SrcDoc,
                        sidebar = extension.Sidebar
                    }
                }), cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<UiExtension>();
        }

        /// <summary>
        /// Creates or updates a UI extension. Updates if an extension with the same id already exists.
        /// </summary>
        /// <param name="extension">The <see cref="Contentful.Core.Models.Management.UiExtension"/> to create or update. **Remember to set the id property.**</param>
        /// <param name="spaceId">The id of the space to create the content type in. Will default to the one set when creating the client.</param>
        /// <param name="version">The last version known of the extension. Must be set for existing extensions. Should be null if one is created.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created or updated <see cref="Contentful.Core.Models.Management.UiExtension"/>.</returns>
        /// <exception cref="ArgumentException">Thrown if the id of the content type is not set.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<UiExtension> CreateOrUpdateExtensionAsync(UiExtension extension, string spaceId = null, int? version = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (extension.SystemProperties?.Id == null)
            {
                throw new ArgumentException("The id of the extension must be set.", nameof(extension));
            }

            AddVersionHeader(version);

            var res = await PutAsync(
                $"{_baseUrl}{spaceId ?? _options.SpaceId}/extensions/{extension.SystemProperties.Id}",
                ConvertObjectToJsonStringContent(new
                {
                    extension = new
                    {
                        src = extension.Src,
                        name = extension.Name,
                        fieldTypes = extension.FieldTypes?.Select(c => new { type = c }),
                        srcDoc = extension.SrcDoc,
                        sidebar = extension.Sidebar
                    }
                }), cancellationToken).ConfigureAwait(false);

            RemoveVersionHeader();

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var json = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return json.ToObject<UiExtension>();
        }

        /// <summary>
        /// Gets a single <see cref="Contentful.Core.Models.Management.UiExtension"/> for a space.
        /// </summary>
        /// <param name="extensionId">The id of the extension to get.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.UiExtension"/>.</returns>
        /// <exception cref="ArgumentException">The <see name="extensionId">extensionId</see> parameter was null or empty.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<UiExtension> GetExtensionAsync(string extensionId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(extensionId))
            {
                throw new ArgumentException("The id of the extension must be set", nameof(extensionId));
            }

            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/extensions/{extensionId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<UiExtension>();
        }

        /// <summary>
        /// Deletes a <see cref="Contentful.Core.Models.Management.UiExtension"/> by the specified id.
        /// </summary>
        /// <param name="extensionId">The id of the extension.</param>
        /// <param name="spaceId">The id of the space to delete the extension in. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The <see name="contentTypeId">contentTypeId</see> parameter was null or empty</exception>
        public async Task DeleteExtensionAsync(string extensionId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(extensionId))
            {
                throw new ArgumentException(nameof(extensionId));
            }

            var res = await DeleteAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/extensions/{extensionId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);
        }


        /// <summary>
        /// Creates a CMA management token that can be used to access the Contentful Management API.
        /// </summary>
        /// <param name="token">The token to create.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.ManagementToken"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ManagementToken> CreateManagementTokenAsync(ManagementToken token, CancellationToken cancellationToken = default(CancellationToken))
        {
            var res = await PostAsync($"{_directApiUrl}users/me/access_tokens",
                ConvertObjectToJsonStringContent(new
                {
                    name = token.Name,
                    scopes = token.Scopes
                }), cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<ManagementToken>();
        }

        /// <summary>
        /// Gets a collection of all <see cref="Contentful.Core.Models.Management.ManagementToken"/> for a user. **Note that the actual token will not be part of the response. 
        /// It is only available directly after creation of a token for security reasons.**
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.UiExtension"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<ManagementToken>> GetAllManagementTokensAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var res = await GetAsync($"{_directApiUrl}users/me/access_tokens", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var collection = jsonObject.ToObject<ContentfulCollection<ManagementToken>>();
            var keys = jsonObject.SelectTokens("$..items[*]").Select(c => c.ToObject<ManagementToken>());
            collection.Items = keys;

            return collection;
        }

        /// <summary>
        /// Gets a single <see cref="Contentful.Core.Models.Management.ManagementToken"/> for a user.
        /// </summary>
        /// <param name="managementTokenId">The id of the management token to get.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.ManagementToken"/>.</returns>
        /// <exception cref="ArgumentException">The <see name="managementTokenId">managementTokenId</see> parameter was null or empty.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ManagementToken> GetManagementTokenAsync(string managementTokenId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(managementTokenId))
            {
                throw new ArgumentException("The id of the token must be set", nameof(managementTokenId));
            }

            var res = await GetAsync($"{_directApiUrl}users/me/access_tokens/{managementTokenId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<ManagementToken>();
        }

        /// <summary>
        /// Revokes a single <see cref="Contentful.Core.Models.Management.ManagementToken"/> for a user.
        /// </summary>
        /// <param name="managementTokenId">The id of the management token to revoke.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The revoked <see cref="Contentful.Core.Models.Management.ManagementToken"/>.</returns>
        /// <exception cref="ArgumentException">The <see name="managementTokenId">managementTokenId</see> parameter was null or empty.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ManagementToken> RevokeManagementTokenAsync(string managementTokenId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(managementTokenId))
            {
                throw new ArgumentException("The id of the token must be set", nameof(managementTokenId));
            }

            var res = await PutAsync($"{_directApiUrl}users/me/access_tokens/{managementTokenId}/revoked", null, cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResultAsync(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<ManagementToken>();
        }


        private async Task<HttpResponseMessage> PostAsync(string url, HttpContent content, CancellationToken cancellationToken)
        {
            return await SendHttpRequestAsync(url, HttpMethod.Post, _options.ManagementApiKey, cancellationToken, content).ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> PutAsync(string url, HttpContent content, CancellationToken cancellationToken)
        {
            return await SendHttpRequestAsync(url, HttpMethod.Put, _options.ManagementApiKey, cancellationToken, content).ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> DeleteAsync(string url, CancellationToken cancellationToken)
        {
            return await SendHttpRequestAsync(url, HttpMethod.Delete, _options.ManagementApiKey, cancellationToken).ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> GetAsync(string url, CancellationToken cancellationToken)
        {
            return await SendHttpRequestAsync(url, HttpMethod.Get, _options.ManagementApiKey, cancellationToken).ConfigureAwait(false);
        }

        private StringContent ConvertObjectToJsonStringContent(object ob)
        {
            var resolver = new CamelCasePropertyNamesContractResolver();
            resolver.NamingStrategy.OverrideSpecifiedNames = false;

            var serializedObject = JsonConvert.SerializeObject(ob, new JsonSerializerSettings
            {
                ContractResolver = resolver

            });
            return new StringContent(serializedObject, Encoding.UTF8, "application/vnd.contentful.management.v1+json");
        }
    }
}
