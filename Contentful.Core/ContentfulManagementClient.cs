using Contentful.Core.Configuration;
using Contentful.Core.Errors;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contentful.Core.Search;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
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
        /// <param name="options">The <see cref="ContentfulOptions"/> used for this client.</param>
        /// <exception cref="ArgumentException">The <see name="options">options</see> parameter was null or empty</exception>
        public ContentfulManagementClient(HttpClient httpClient, ContentfulOptions options)
        {
            _httpClient = httpClient;
            _options = options;

            if (options == null)
            {
                throw new ArgumentException("The ContentfulOptions cannot be null.", nameof(options));
            }

            SerializerSettings.Converters.Add(new ExtensionJsonConverter());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentfulManagementClient"/> class.
        /// </summary>
        /// <param name="httpClient">The HttpClient of your application.</param>
        /// <param name="managementApiKey">The management API key used when communicating with the Contentful API</param>
        /// <param name="spaceId">The id of the space to fetch content from.</param>
        public ContentfulManagementClient(HttpClient httpClient, string managementApiKey, string spaceId) :
            this(httpClient, new ContentfulOptions()
            {
                ManagementApiKey = managementApiKey,
                SpaceId = spaceId
            })
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
        public async Task<Space> CreateSpace(string name, string defaultLocale, string organisation = null, CancellationToken cancellationToken = default)
        {
            var res = await PostAsync(_baseUrl, ConvertObjectToJsonStringContent(new { name, defaultLocale }), cancellationToken, null, organisationId: organisation).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var json = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return json.ToObject<Space>(Serializer);
        }

        /// <summary>
        /// Updates the name of a space in Contentful.
        /// </summary>
        /// <param name="space">The space to update, needs to contain at minimum name, Id and version.</param>
        /// <param name="organisation">The organisation to update a space for. Not required if the account belongs to only one organisation.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The updated <see cref="Space"/></returns>
        public async Task<Space> UpdateSpaceName(Space space, string organisation = null, CancellationToken cancellationToken = default)
        {
            return await UpdateSpaceName(space.SystemProperties.Id, space.Name, space.SystemProperties.Version ?? 1, organisation, cancellationToken).ConfigureAwait(false);
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
        public async Task<Space> UpdateSpaceName(string id, string name, int version, string organisation = null, CancellationToken cancellationToken = default)
        {
            var res = await PutAsync($"{_baseUrl}{id}", ConvertObjectToJsonStringContent(new { name }), cancellationToken, version, organisationId: organisation).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var json = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return json.ToObject<Space>(Serializer);
        }

        /// <summary>
        /// Gets a space in Contentful.
        /// </summary>
        /// <param name="id">The id of the space to get.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Space" /></returns>
        public async Task<Space> GetSpace(string id, CancellationToken cancellationToken = default)
        {
            var res = await GetAsync($"{_baseUrl}{id}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var json = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return json.ToObject<Space>(Serializer);
        }

        /// <summary>
        /// Gets all spaces in Contentful.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Space"/>.</returns>
        public async Task<IEnumerable<Space>> GetSpaces(CancellationToken cancellationToken = default)
        {
            var res = await GetAsync(_baseUrl, cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var json = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return json.SelectTokens("$..items[*]").Select(t => t.ToObject<Space>(Serializer));
        }

        /// <summary>
        /// Deletes a space in Contentful.
        /// </summary>
        /// <param name="id">The id of the space to delete.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns></returns>
        public async Task DeleteSpace(string id, CancellationToken cancellationToken = default)
        {
            var res = await DeleteAsync($"{_baseUrl}{id}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);
        }

        /// <summary>
        /// Get all content types of a space.
        /// </summary>
        /// <param name="spaceId">The id of the space to get the content types of. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ContentType"/>.</returns>
        public async Task<IEnumerable<ContentType>> GetContentTypes(string spaceId = null, CancellationToken cancellationToken = default)
        {
            return await GetContentTypes(null, spaceId, cancellationToken);
        }

        /// <summary>
        /// Get all content types of a space.
        /// </summary>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <param name="spaceId">The id of the space to get the content types of. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ContentType"/>.</returns>
        public async Task<IEnumerable<ContentType>> GetContentTypes(string queryString, string spaceId = null, CancellationToken cancellationToken = default)
        {
            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}content_types/{queryString}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var json = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return json.SelectTokens("$..items[*]").Select(t => t.ToObject<ContentType>(Serializer));
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
        public async Task<ContentType> CreateOrUpdateContentType(ContentType contentType, string spaceId = null, int? version = null, CancellationToken cancellationToken = default)
        {
            if (contentType.SystemProperties?.Id == null)
            {
                throw new ArgumentException("The id of the content type must be set.", nameof(contentType));
            }

            var res = await PutAsync(
                $"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}content_types/{contentType.SystemProperties.Id}",
                ConvertObjectToJsonStringContent(new { name = contentType.Name, description = contentType.Description, displayField = contentType.DisplayField, fields = contentType.Fields }), 
                cancellationToken, version).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var json = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return json.ToObject<ContentType>(Serializer);
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
        public async Task<ContentType> GetContentType(string contentTypeId, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(contentTypeId))
            {
                throw new ArgumentException(nameof(contentTypeId));
            }

            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}content_types/{contentTypeId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var contentType = jsonObject.ToObject<ContentType>(Serializer);

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
        public async Task DeleteContentType(string contentTypeId, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(contentTypeId))
            {
                throw new ArgumentException(nameof(contentTypeId));
            }

            var res = await DeleteAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}content_types/{contentTypeId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);
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
        public async Task<ContentType> ActivateContentType(string contentTypeId, int version, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(contentTypeId))
            {
                throw new ArgumentException(nameof(contentTypeId));
            }

            var res = await PutAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}content_types/{contentTypeId}/published", null, cancellationToken, version).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var contentType = jsonObject.ToObject<ContentType>(Serializer);

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
        public async Task DeactivateContentType(string contentTypeId, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(contentTypeId))
            {
                throw new ArgumentException(nameof(contentTypeId));
            }

            var res = await DeleteAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}content_types/{contentTypeId}/published", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);
        }

        /// <summary>
        /// Get all activated content types of a space.
        /// </summary>
        /// <param name="spaceId">The id of the space to get the activated content types of. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ContentType"/>.</returns>
        public async Task<IEnumerable<ContentType>> GetActivatedContentTypes(string spaceId = null, CancellationToken cancellationToken = default)
        {
            return await GetActivatedContentTypes(null, spaceId, cancellationToken);
        }

        /// <summary>
        /// Get all activated content types of a space.
        /// </summary>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <param name="spaceId">The id of the space to get the activated content types of. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ContentType"/>.</returns>
        public async Task<IEnumerable<ContentType>> GetActivatedContentTypes(string queryString, string spaceId = null, CancellationToken cancellationToken = default)
        {
            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}public/content_types/{queryString}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var json = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return json.SelectTokens("$..items[*]").Select(t => t.ToObject<ContentType>(Serializer));
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
        public async Task<EditorInterface> GetEditorInterface(string contentTypeId, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(contentTypeId))
            {
                throw new ArgumentException(nameof(contentTypeId));
            }

            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}content_types/{contentTypeId}/editor_interface", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var editorInterface = jsonObject.ToObject<EditorInterface>(Serializer);

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
        public async Task<EditorInterface> UpdateEditorInterface(EditorInterface editorInterface, string contentTypeId, int version, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(contentTypeId))
            {
                throw new ArgumentException(nameof(contentTypeId));
            }

            var res = await PutAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}content_types/{contentTypeId}/editor_interface",
                ConvertObjectToJsonStringContent(new { controls = editorInterface.Controls }), cancellationToken, version).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var updatedEditorInterface = jsonObject.ToObject<EditorInterface>(Serializer);

            return updatedEditorInterface;
        }

        /// <summary>
        /// Gets all the entries of a space in a specific locale, filtered by an optional <see cref="QueryBuilder{T}"/>.
        /// </summary>
        /// <param name="queryBuilder">The optional <see cref="QueryBuilder{T}"/> to add additional filtering to the query.</param>
        /// <param name="locale">The locale to fetch entries for. Defaults to the default of the space.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of items.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<T>> GetEntriesForLocale<T>(QueryBuilder<T> queryBuilder, string locale = null, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(locale))
            {
                locale = (await GetLocalesCollection(spaceId, cancellationToken)).FirstOrDefault(c => c.Default).Code;
            }

            var entries = await GetEntriesCollection<JObject>(queryBuilder?.Build(), cancellationToken);

            var items = entries.Items.Select(j => ReconstructJsonObject(j).ToObject<T>()).ToList();

            var collection = new ContentfulCollection<T>();
            collection.Limit = entries.Limit;
            collection.Skip = entries.Skip;
            collection.Total = entries.Total;
            collection.Errors = entries.Errors;
            collection.IncludedAssets = entries.IncludedAssets;
            collection.IncludedEntries = entries.IncludedEntries;
            collection.Items = items;

            return collection;

            JObject ReconstructJsonObject(JObject oldObject)
            {
                var newObject = new JObject(new JProperty("sys", oldObject["sys"]));
                foreach(var child in oldObject.Children<JProperty>().Where(p => p.Name != "sys"))
                {
                    var value = child.Value[locale];
                    newObject.Add(child.Name, value);
                }

                return newObject;
            }
        }

        /// <summary>
        /// Gets all the entries of a space, filtered by an optional <see cref="QueryBuilder{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type to serialize the response into.</typeparam>
        /// <param name="queryBuilder">The optional <see cref="QueryBuilder{T}"/> to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of items.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<T>> GetEntriesCollection<T>(QueryBuilder<T> queryBuilder, CancellationToken cancellationToken = default)
        {
            return await GetEntriesCollection<T>(queryBuilder?.Build(), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all the entries of a space, filtered by an optional querystring. A simpler approach than 
        /// to construct a query manually is to use the <see cref="QueryBuilder{T}"/> class.
        /// </summary>
        /// <typeparam name="T">The type to serialize the response into.</typeparam>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of items.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<T>> GetEntriesCollection<T>(string queryString = null, CancellationToken cancellationToken = default)
        {
            var res = await GetAsync($"{_baseUrl}{_options.SpaceId}/{EnvironmentsBase}entries{queryString}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var isContentfulResource = typeof(IContentfulResource).GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo());

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            var collection = jsonObject.ToObject<ContentfulCollection<T>>(Serializer);

            if (!isContentfulResource)
            {
                var entryTokens = jsonObject.SelectTokens("$.items[*]..fields").ToList();

                for (var i = entryTokens.Count - 1; i >= 0; i--)
                {
                    var token = entryTokens[i];
                    var grandParent = token.Parent.Parent;

                    if (grandParent["sys"]?["type"] != null && grandParent["sys"]["type"]?.ToString() != "Entry")
                    {
                        continue;
                    }

                    //Remove the fields property and let the fields be direct descendants of the node to make deserialization logical.
                    token.Parent.Remove();
                    grandParent.Add(token.Children());
                }

                var entries = jsonObject.SelectToken("$.items").ToObject<IEnumerable<T>>(Serializer);
                collection.Items = entries;
            }

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
        public async Task<Entry<dynamic>> CreateEntry(Entry<dynamic> entry, string contentTypeId, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(contentTypeId))
            {
                throw new ArgumentException("The content type id must be set.", nameof(contentTypeId));
            }

            var res = await PostAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}entries",
                ConvertObjectToJsonStringContent(new { fields = entry.Fields }), cancellationToken, null, contentTypeId).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var updatedEntry = jsonObject.ToObject<Entry<dynamic>>(Serializer);

            return updatedEntry;
        }

        /// <summary>
        /// Creates an entry.
        /// </summary>
        /// <param name="entry">The object to create an entry from.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="contentTypeId">The id of the <see cref="ContentType"/> of the entry.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created entry.</returns>
        public async Task<T> CreateEntry<T>(T entry, string contentTypeId, string spaceId = null, CancellationToken cancellationToken = default)
        {
            var entryToCreate = new Entry<dynamic>
            {
                Fields = entry
            };

            var createdEntry = await CreateEntry(entryToCreate, contentTypeId, spaceId, cancellationToken);
            return (createdEntry.Fields as JObject).ToObject<T>();
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
        public async Task<Entry<dynamic>> CreateOrUpdateEntry(Entry<dynamic> entry, string spaceId = null, string contentTypeId = null, int? version = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(entry.SystemProperties?.Id))
            {
                throw new ArgumentException("The id of the entry must be set.");
            }

            var res = await PutAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}entries/{entry.SystemProperties.Id}",
                ConvertObjectToJsonStringContent(new { fields = entry.Fields }), cancellationToken, version, contentTypeId).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var updatedEntry = jsonObject.ToObject<Entry<dynamic>>(Serializer);

            return updatedEntry;
        }

        /// <summary>
        /// Creates or updates an entry. Updates if an entry with the same id already exists.
        /// </summary>
        /// <param name="entry">The entry to create or update.</param>
        /// <param name="id">The id of the entry to create or update.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="contentTypeId">The id of the <see cref="ContentType"/> of the entry. Need only be set if you are creating a new entry.</param>
        /// <param name="version">The last known version of the entry. Must be set when updating an entry.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created or updated entry.</returns>
        public async Task<T> CreateOrUpdateEntry<T>(T entry, string id, string spaceId = null, string contentTypeId = null, int? version = null, CancellationToken cancellationToken = default)
        {
            var entryToCreate = new Entry<dynamic>
            {
                SystemProperties = new SystemProperties
                {
                    Id = id
                },
                Fields = entry
            };

            var createdEntry = await CreateOrUpdateEntry(entryToCreate, spaceId, contentTypeId, version, cancellationToken);
            return (createdEntry.Fields as JObject).ToObject<T>();
        }

        /// <summary>
        /// Creates an entry with values for a certain locale from the provided object.
        /// </summary>
        /// <param name="entry">The object to use as values for the entry fields.</param>
        /// <param name="id">The of the entry to create.</param>
        /// <param name="contentTypeId">The id of the content type to create an entry for.</param>
        /// <param name="locale">The locale to set fields for. The default locale for the space will be used if this parameter is null or empty.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Entry{T}"/>.</returns>
        public async Task<Entry<dynamic>> CreateEntryForLocale(object entry, string id, string contentTypeId, string locale = null, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(locale))
            {
                locale = (await GetLocalesCollection(spaceId, cancellationToken)).FirstOrDefault(c => c.Default).Code;
            }

            var jsonEntry = JObject.Parse(ConvertObjectToJsonString(entry));
            var jsonToCreate = new JObject();
            foreach (var prop in jsonEntry.Children().Where(p => p is JProperty).Cast<JProperty>())
            {
                var val = jsonEntry[prop.Name];
                jsonToCreate.Add(new JProperty(prop.Name, new JObject(new JProperty(locale, val))));
            }

            var entryToCreate = new Entry<dynamic>
            {
                SystemProperties = new SystemProperties
                {
                    Id = id
                },
                Fields = jsonToCreate
            };

            return await CreateOrUpdateEntry(entryToCreate, spaceId: spaceId, contentTypeId: contentTypeId, cancellationToken: cancellationToken);
        }


        /// <summary>
        /// Updates an entry fields for a certain locale using the values from the provided object.
        /// </summary>
        /// <param name="entry">The object to use as values for the entry fields.</param>
        /// <param name="id">The id of the entry to update.</param>
        /// <param name="locale">The locale to set the fields for. The default locale for the space will be used if this parameter is null or empty.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The updated <see cref="Entry{T}"/>.</returns>
        public async Task<Entry<dynamic>> UpdateEntryForLocale(object entry, string id, string locale = null, string spaceId = null, CancellationToken cancellationToken = default)
        {
            var entryToUpdate = await GetEntry(id, spaceId);
            var contentType = await GetContentType(entryToUpdate.SystemProperties.ContentType.SystemProperties.Id);
            var allFieldIds = contentType.Fields.Select(f => f.Id);

            if(string.IsNullOrEmpty(locale))
            {
                locale = (await GetLocalesCollection(spaceId, cancellationToken)).FirstOrDefault(c => c.Default).Code;
            }

            var jsonEntry = JObject.Parse(ConvertObjectToJsonString(entry));
            var fieldsToUpdate = (entryToUpdate.Fields as JObject);

            foreach (var fieldId in allFieldIds)
            {
                if(jsonEntry[fieldId] != null)
                {
                    if(fieldsToUpdate[fieldId] == null)
                    {
                        fieldsToUpdate.Add(fieldId, new JObject(new JProperty(locale, null)));
                    }

                    fieldsToUpdate[fieldId][locale] = jsonEntry[fieldId];
                }
            }

            var updatedEntry = await CreateOrUpdateEntry(entryToUpdate,spaceId: spaceId, version: entryToUpdate.SystemProperties.Version, cancellationToken: cancellationToken);

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
        public async Task<Entry<dynamic>> GetEntry(string entryId, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(entryId))
            {
                throw new ArgumentException(nameof(entryId));
            }

            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}entries/{entryId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            return JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false)).ToObject<Entry<dynamic>>(Serializer);
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
        public async Task DeleteEntry(string entryId, int version, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(entryId))
            {
                throw new ArgumentException(nameof(entryId));
            }

            var res = await DeleteAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}entries/{entryId}", cancellationToken, version).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);
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
        public async Task<Entry<dynamic>> PublishEntry(string entryId, int version, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(entryId))
            {
                throw new ArgumentException(nameof(entryId));
            }

            var res = await PutAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}entries/{entryId}/published", null, cancellationToken, version).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            return JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false)).ToObject<Entry<dynamic>>(Serializer);
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
        public async Task<Entry<dynamic>> UnpublishEntry(string entryId, int version, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(entryId))
            {
                throw new ArgumentException(nameof(entryId));
            }

            var res = await DeleteAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}entries/{entryId}/published", cancellationToken, version).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            return JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false)).ToObject<Entry<dynamic>>(Serializer);
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
        public async Task<Entry<dynamic>> ArchiveEntry(string entryId, int version, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(entryId))
            {
                throw new ArgumentException(nameof(entryId));
            }

            var res = await PutAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}entries/{entryId}/archived", null, cancellationToken, version).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            return JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false)).ToObject<Entry<dynamic>>(Serializer);
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
        public async Task<Entry<dynamic>> UnarchiveEntry(string entryId, int version, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(entryId))
            {
                throw new ArgumentException(nameof(entryId));
            }

            var res = await DeleteAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}entries/{entryId}/archived", cancellationToken, version).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            return JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false)).ToObject<Entry<dynamic>>(Serializer);
        }

        /// <summary>
        /// Gets all assets of a space, filtered by an optional <see cref="QueryBuilder{T}"/>.
        /// </summary>
        /// <param name="queryBuilder">The optional <see cref="QueryBuilder{T}"/> to add additional filtering to the query.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.ManagementAsset"/>.</returns>
        /// <exception cref="Contentful.Core.Errors.ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<ManagementAsset>> GetAssetsCollection(QueryBuilder<Asset> queryBuilder, string spaceId = null, CancellationToken cancellationToken = default)
        {
            return await GetAssetsCollection(queryBuilder?.Build(), spaceId, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all assets in the space.
        /// </summary>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.ManagementAsset"/>.</returns>
        /// <exception cref="Contentful.Core.Errors.ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<ManagementAsset>> GetAssetsCollection(string queryString = null, string spaceId = null, CancellationToken cancellationToken = default)
        {
            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}assets/{queryString}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var collection = jsonObject.ToObject<ContentfulCollection<ManagementAsset>>(Serializer);
            var assets = jsonObject.SelectTokens("$..items[*]").Select(c => c.ToObject<ManagementAsset>(Serializer));
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
        public async Task<ContentfulCollection<ManagementAsset>> GetPublishedAssetsCollection(string spaceId = null, CancellationToken cancellationToken = default)
        {
            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}public/assets", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var collection = jsonObject.ToObject<ContentfulCollection<ManagementAsset>>(Serializer);
            var assets = jsonObject.SelectTokens("$..items[*]").Select(c => c.ToObject<ManagementAsset>(Serializer));
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
        public async Task<ManagementAsset> GetAsset(string assetId, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(assetId))
            {
                throw new ArgumentException(nameof(assetId));
            }

            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}assets/{assetId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<ManagementAsset>(Serializer);
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
        public async Task DeleteAsset(string assetId, int version, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(assetId))
            {
                throw new ArgumentException(nameof(assetId));
            }

            var res = await DeleteAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}assets/{assetId}", cancellationToken, version).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);
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
        public async Task<ManagementAsset> PublishAsset(string assetId, int version, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(assetId))
            {
                throw new ArgumentException(nameof(assetId));
            }

            var res = await PutAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}assets/{assetId}/published", null, cancellationToken, version).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<ManagementAsset>(Serializer);
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
        public async Task<ManagementAsset> UnpublishAsset(string assetId, int version, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(assetId))
            {
                throw new ArgumentException(nameof(assetId));
            }

            var res = await DeleteAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}assets/{assetId}/published", cancellationToken, version).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<ManagementAsset>(Serializer);
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
        public async Task<ManagementAsset> ArchiveAsset(string assetId, int version, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(assetId))
            {
                throw new ArgumentException(nameof(assetId));
            }

            HttpResponseMessage res = null;

            res = await PutAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}assets/{assetId}/archived", null, cancellationToken, version).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<ManagementAsset>(Serializer);
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
        public async Task<ManagementAsset> UnarchiveAsset(string assetId, int version, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(assetId))
            {
                throw new ArgumentException(nameof(assetId));
            }

            var res = await DeleteAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}assets/{assetId}/archived", cancellationToken, version).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<ManagementAsset>(Serializer);
        }

        /// <summary>
        /// Processes an asset by the specified id and keeps polling the API until it has finished processing. **Note that this might result in multiple API calls.**
        /// </summary>
        /// <param name="assetId">The id of the asset to process.</param>
        /// <param name="version">The last known version of the asset.</param>
        /// <param name="locale">The locale for which files should be processed.</param>
        /// <param name="maxDelay">The maximum number of milliseconds allowed for the operation.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.ManagementAsset"/> that has been processed.</returns>
        /// <exception cref="ArgumentException">The <see name="assetId">assetId</see> parameter was null or empty.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="TimeoutException">The processing of the asset did not finish within the allotted time.</exception>
        public async Task<ManagementAsset> ProcessAssetUntilCompleted(string assetId, int version, string locale, int maxDelay = 2000, string spaceId = null, CancellationToken cancellationToken = default)
        {
            await ProcessAsset(assetId, version, locale, spaceId, cancellationToken);

            var processedAsset = await GetAsset(assetId, spaceId, cancellationToken);
            var delay = 0;
            var completed = false;
            
            while (completed == false && delay < maxDelay)
            {
                await Task.Delay(delay);

                if (processedAsset?.Files[locale]?.Url == null)
                {
                    processedAsset = await GetAsset(assetId, spaceId, cancellationToken);
                }
                else
                {
                    return processedAsset;
                }
                
                delay += 200;
            }

            throw new TimeoutException($"The processing of the asset did not finish in a timely manner. Max delay of {maxDelay} reached.");
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
        public async Task ProcessAsset(string assetId, int version, string locale, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(assetId))
            {
                throw new ArgumentException(nameof(assetId));
            }

            HttpResponseMessage res = null;

            res = await PutAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}assets/{assetId}/files/{locale}/process", null, cancellationToken, version).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);
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
        public async Task<ManagementAsset> CreateOrUpdateAsset(ManagementAsset asset, string spaceId = null, int? version = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(asset.SystemProperties?.Id))
            {
                throw new ArgumentException("The id of the asset must be set.");
            }

            var res = await PutAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}assets/{asset.SystemProperties.Id}",
                ConvertObjectToJsonStringContent(new { fields = new { title = asset.Title, description = asset.Description, file = asset.Files } }), cancellationToken, version).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var updatedAsset = jsonObject.ToObject<ManagementAsset>(Serializer);

            return updatedAsset;
        }

        /// <summary>
        /// Creates an <see cref="Contentful.Core.Models.Management.ManagementAsset"/> with a randomly created id.
        /// </summary>
        /// <param name="asset">The asset to create.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.ManagementAsset"/></returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ManagementAsset> CreateAsset(ManagementAsset asset, string spaceId = null, CancellationToken cancellationToken = default)
        {
            var res = await PostAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}assets",
                ConvertObjectToJsonStringContent(new { fields = new { title = asset.Title, description = asset.Description, file = asset.Files } }), cancellationToken, null).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var createdAsset = jsonObject.ToObject<ManagementAsset>(Serializer);

            return createdAsset;
        }

        /// <summary>
        /// Gets all locales in a <see cref="Space"/>.
        /// </summary>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{Locale}"/> of locales.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<Locale>> GetLocalesCollection(string spaceId = null, CancellationToken cancellationToken = default)
        {
            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}locales", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var collection = jsonObject.ToObject<ContentfulCollection<Locale>>(Serializer);
            var locales = jsonObject.SelectTokens("$..items[*]").Select(c => c.ToObject<Locale>(Serializer));
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
        public async Task<Locale> CreateLocale(Locale locale, string spaceId = null, CancellationToken cancellationToken = default)
        {
            var res = await PostAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}locales",
                ConvertObjectToJsonStringContent(
                    new
                    {
                        code = locale.Code,
                        contentDeliveryApi = locale.ContentDeliveryApi,
                        contentManagementApi = locale.ContentManagementApi,
                        fallbackCode = locale.FallbackCode,
                        name = locale.Name,
                        optional = locale.Optional
                    }), cancellationToken, null).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<Locale>(Serializer);
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
        public async Task<Locale> GetLocale(string localeId, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(localeId))
            {
                throw new ArgumentException("The localeId must be set.");
            }

            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}locales/{localeId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<Locale>(Serializer);
        }

        /// <summary>
        /// Updates a locale in the specified <see cref="Space"/>.
        /// </summary>
        /// <param name="locale">The <see cref="Contentful.Core.Models.Management.Locale"/> to update.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.Locale"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<Locale> UpdateLocale(Locale locale, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(locale.SystemProperties?.Id))
            {
                throw new ArgumentException("The id of the Locale must be set.");
            }

            var res = await PutAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}locales/{locale.SystemProperties.Id}", ConvertObjectToJsonStringContent(new
            {
                code = locale.Code,
                contentDeliveryApi = locale.ContentDeliveryApi,
                contentManagementApi = locale.ContentManagementApi,
                fallbackCode = locale.FallbackCode,
                name = locale.Name,
                optional = locale.Optional
            }), cancellationToken, locale.SystemProperties.Version).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<Locale>(Serializer);
        }

        /// <summary>
        /// Deletes a locale by the specified id.
        /// </summary>
        /// <param name="localeId">The id of the locale to delete.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <exception cref="ArgumentException">The <see name="localeId">localeId</see> parameter was null or empty.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task DeleteLocale(string localeId, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(localeId))
            {
                throw new ArgumentException("The localeId must be set.");
            }

            var res = await DeleteAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}locales/{localeId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all webhooks for a <see cref="Space"/>.
        /// </summary>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.Webhook"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<Webhook>> GetWebhooksCollection(string spaceId = null, CancellationToken cancellationToken = default)
        {
            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/webhook_definitions", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var collection = jsonObject.ToObject<ContentfulCollection<Webhook>>(Serializer);
            var hooks = jsonObject.SelectTokens("$..items[*]").Select(c => c.ToObject<Webhook>(Serializer));
            collection.Items = hooks;

            return collection;
        }

        /// <summary>
        /// Creates a webhook in a <see cref="Space"/>.
        /// </summary>
        /// <param name="webhook">The webhook to create.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.Webhook"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<Webhook> CreateWebhook(Webhook webhook, string spaceId = null, CancellationToken cancellationToken = default)
        {
            //Not allowed to post system properties
            webhook.SystemProperties = null;

            var res = await PostAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/webhook_definitions", ConvertObjectToJsonStringContent(webhook), cancellationToken, null).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<Webhook>(Serializer);
        }

        /// <summary>
        /// Creates or updates a webhook in a <see cref="Space"/>.  Updates if a webhook with the same id already exists.
        /// </summary>
        /// <param name="webhook">The webhook to create or update.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="version">The last known version of the webhook.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.Webhook"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The id of the webhook parameter was null or empty.</exception>
        public async Task<Webhook> CreateOrUpdateWebhook(Webhook webhook, string spaceId = null, int? version = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(webhook?.SystemProperties?.Id))
            {
                throw new ArgumentException("The id of the webhook must be set.");
            }

            var id = webhook.SystemProperties.Id;

            //Not allowed to post system properties
            webhook.SystemProperties = null;

            var res = await PutAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/webhook_definitions/{id}", ConvertObjectToJsonStringContent(webhook), cancellationToken, version).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<Webhook>(Serializer);
        }

        /// <summary>
        /// Gets a single webhook from a <see cref="Space"/>.
        /// </summary>
        /// <param name="webhookId">The id of the webhook to get.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.Webhook"/>.</returns>
        /// <exception cref="ArgumentException">The <see name="webhookId">webhookId</see> parameter was null or empty.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<Webhook> GetWebhook(string webhookId, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(webhookId))
            {
                throw new ArgumentException("The id of the webhook must be set.", nameof(webhookId));
            }

            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/webhook_definitions/{webhookId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<Webhook>(Serializer);
        }

        /// <summary>
        /// Deletes a webhook from a <see cref="Space"/>.
        /// </summary>
        /// <param name="webhookId">The id of the webhook to delete.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <exception cref="ArgumentException">The <see name="webhookId">webhookId</see> parameter was null or empty.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task DeleteWebhook(string webhookId, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(webhookId))
            {
                throw new ArgumentException("The id of the webhook must be set", nameof(webhookId));
            }

            var res = await DeleteAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/webhook_definitions/{webhookId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all recent call details for a webhook.
        /// </summary>
        /// <param name="webhookId">The id of the webhook to get details for.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.WebhookCallDetails"/>.</returns>
        /// <exception cref="ArgumentException">The <see name="webhookId">webhookId</see> parameter was null or empty.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<WebhookCallDetails>> GetWebhookCallDetailsCollection(string webhookId, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(webhookId))
            {
                throw new ArgumentException("The id of the webhook must be set.", nameof(webhookId));
            }

            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/webhooks/{webhookId}/calls", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var collection = jsonObject.ToObject<ContentfulCollection<WebhookCallDetails>>(Serializer);
            var hooks = jsonObject.SelectTokens("$..items[*]").Select(c => c.ToObject<WebhookCallDetails>(Serializer));
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
        /// <returns>The <see cref="Contentful.Core.Models.Management.WebhookCallDetails"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The <see name="webhookId">webhookId</see> or <see name="callId">callId</see> parameter was null or empty.</exception>
        public async Task<WebhookCallDetails> GetWebhookCallDetails(string callId, string webhookId, string spaceId = null, CancellationToken cancellationToken = default)
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

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<WebhookCallDetails>(Serializer);
        }

        /// <summary>
        /// Gets a response containing an overview of the recent webhook calls.
        /// </summary>
        /// <param name="webhookId">The id of the webhook to get health details for.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="Contentful.Core.Models.Management.WebhookHealthResponse"/>.</returns>
        /// <exception cref="ArgumentException">The <see name="webhookId">webhookId</see> parameter was null or empty.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<WebhookHealthResponse> GetWebhookHealth(string webhookId, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(webhookId))
            {
                throw new ArgumentException("The id of the webhook must be set.", nameof(webhookId));
            }

            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/webhooks/{webhookId}/health", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var health = new WebhookHealthResponse()
            {
                SystemProperties = jsonObject["sys"]?.ToObject<SystemProperties>(Serializer),
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
        public async Task<Role> GetRole(string roleId, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                throw new ArgumentException("The id of the role must be set", nameof(roleId));
            }

            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/roles/{roleId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<Role>(Serializer);
        }

        /// <summary>
        /// Gets all <see cref="Contentful.Core.Models.Management.Role">roles</see> of a space.
        /// </summary>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.Role"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<Role>> GetAllRoles(string spaceId = null, CancellationToken cancellationToken = default)
        {
            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/roles", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var collection = jsonObject.ToObject<ContentfulCollection<Role>>(Serializer);
            var roles = jsonObject.SelectTokens("$..items[*]").Select(c => c.ToObject<Role>(Serializer));
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
        public async Task<Role> CreateRole(Role role, string spaceId = null, CancellationToken cancellationToken = default)
        {
            //Not allowed to post system properties
            role.SystemProperties = null;

            var res = await PostAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/roles", ConvertObjectToJsonStringContent(role), cancellationToken, null).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<Role>(Serializer);
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
        public async Task<Role> UpdateRole(Role role, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(role?.SystemProperties?.Id))
            {
                throw new ArgumentException("The id of the role must be set.");
            }

            var id = role.SystemProperties.Id;

            //Not allowed to post system properties
            role.SystemProperties = null;

            var res = await PutAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/roles/{id}", ConvertObjectToJsonStringContent(role), cancellationToken, null).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<Role>(Serializer);
        }

        /// <summary>
        /// Deletes a role by the specified id.
        /// </summary>
        /// <param name="roleId">The id of the role to delete.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <exception cref="ArgumentException">The <see name="roleId">roleId</see> parameter was null or empty.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task DeleteRole(string roleId, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                throw new ArgumentException("The id of the role must be set", nameof(roleId));
            }

            var res = await DeleteAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/roles/{roleId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all snapshots for an <see cref="Entry{T}"/>.
        /// </summary>
        /// <param name="entryId">The id of the entry to get snapshots for.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A collection of <see cref="Contentful.Core.Models.Management.Snapshot"/>.</returns>
        public async Task<ContentfulCollection<Snapshot>> GetAllSnapshotsForEntry(string entryId, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(entryId))
            {
                throw new ArgumentException("The id of the entry must be set", nameof(entryId));
            }

            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}entries/{entryId}/snapshots", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var collection = jsonObject.ToObject<ContentfulCollection<Snapshot>>(Serializer);
            var snapshots = jsonObject.SelectTokens("$..items[*]").Select(c => c.ToObject<Snapshot>(Serializer));
            collection.Items = snapshots;

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
        public async Task<Snapshot> GetSnapshotForEntry(string snapshotId, string entryId, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(snapshotId))
            {
                throw new ArgumentException("The id of the snapshot must be set.", nameof(snapshotId));
            }

            if (string.IsNullOrEmpty(entryId))
            {
                throw new ArgumentException("The id of the entry must be set.", nameof(entryId));
            }

            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}entries/{entryId}/snapshots/{snapshotId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<Snapshot>(Serializer);
        }

        /// <summary>
        /// Gets all snapshots for a <see cref="ContentType"/>.
        /// </summary>
        /// <param name="contentTypeId">The id of the content type to get snapshots for.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A collection of <see cref="Contentful.Core.Models.Management.SnapshotContentType"/>.</returns>
        public async Task<ContentfulCollection<SnapshotContentType>> GetAllSnapshotsForContentType(string contentTypeId, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(contentTypeId))
            {
                throw new ArgumentException("The id of the content type must be set.", nameof(contentTypeId));
            }

            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}content_types/{contentTypeId}/snapshots", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var collection = jsonObject.ToObject<ContentfulCollection<SnapshotContentType>>(Serializer);
            var snapshots = jsonObject.SelectTokens("$..items[*]").Select(c => c.ToObject<SnapshotContentType>(Serializer));
            collection.Items = snapshots;

            return collection;
        }

        /// <summary>
        /// Gets a single snapshot for a <see cref="ContentType"/>
        /// </summary>
        /// <param name="snapshotId">The id of the snapshot to get.</param>
        /// <param name="contentTypeId">The id of content type the snapshot belongs to.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.SnapshotContentType"/>.</returns>
        public async Task<SnapshotContentType> GetSnapshotForContentType(string snapshotId, string contentTypeId, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(snapshotId))
            {
                throw new ArgumentException("The id of the snapshot must be set.", nameof(snapshotId));
            }

            if (string.IsNullOrEmpty(contentTypeId))
            {
                throw new ArgumentException("The id of the content type must be set.", nameof(contentTypeId));
            }

            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}content_types/{contentTypeId}/snapshots/{snapshotId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<SnapshotContentType>(Serializer);
        }


        /// <summary>
        /// Gets a collection of <see cref="Contentful.Core.Models.Management.SpaceMembership"/> for the user.
        /// </summary>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A collection of <see cref="Contentful.Core.Models.Management.SpaceMembership"/>.</returns>
        public async Task<ContentfulCollection<SpaceMembership>> GetSpaceMemberships(string spaceId = null, CancellationToken cancellationToken = default)
        {
            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/space_memberships", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var collection = jsonObject.ToObject<ContentfulCollection<SpaceMembership>>(Serializer);
            var memberships = jsonObject.SelectTokens("$..items[*]").Select(c => c.ToObject<SpaceMembership>(Serializer));
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
        public async Task<SpaceMembership> CreateSpaceMembership(SpaceMembership spaceMembership, string spaceId = null, CancellationToken cancellationToken = default)
        {
            var res = await PostAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/space_memberships", ConvertObjectToJsonStringContent(spaceMembership), cancellationToken, null).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<SpaceMembership>(Serializer);
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
        public async Task<SpaceMembership> GetSpaceMembership(string spaceMembershipId, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(spaceMembershipId))
            {
                throw new ArgumentException("The id of the space membership must be set", nameof(spaceMembershipId));
            }

            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/space_memberships/{spaceMembershipId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<SpaceMembership>(Serializer);
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
        public async Task<SpaceMembership> UpdateSpaceMembership(SpaceMembership spaceMembership, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(spaceMembership?.SystemProperties?.Id))
            {
                throw new ArgumentException("The id of the space membership id must be set", nameof(spaceMembership));
            }

            var res = await PutAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/space_memberships/{spaceMembership.SystemProperties.Id}", 
                ConvertObjectToJsonStringContent(spaceMembership), cancellationToken, null).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<SpaceMembership>(Serializer);
        }

        /// <summary>
        /// Deletes a <see cref="Contentful.Core.Models.Management.SpaceMembership"/> for a space.
        /// </summary>
        /// <param name="spaceMembershipId">The id of the space membership to delete.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <exception cref="ArgumentException">The <see name="spaceMembershipId">spaceMembershipId</see> parameter was null or empty.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task DeleteSpaceMembership(string spaceMembershipId, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(spaceMembershipId))
            {
                throw new ArgumentException("The id of the space membership must be set", nameof(spaceMembershipId));
            }

            var res = await DeleteAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/space_memberships/{spaceMembershipId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a collection of all <see cref="Contentful.Core.Models.Management.ApiKey"/> in a space.
        /// </summary>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.ApiKey"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<ApiKey>> GetAllApiKeys(string spaceId = null, CancellationToken cancellationToken = default)
        {
            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/api_keys", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var collection = jsonObject.ToObject<ContentfulCollection<ApiKey>>(Serializer);
            var keys = jsonObject.SelectTokens("$..items[*]").Select(c => c.ToObject<ApiKey>(Serializer));
            collection.Items = keys;

            return collection;
        }

        /// <summary>
        /// Gets a collection of all preview <see cref="Contentful.Core.Models.Management.ApiKey"/> in a space.
        /// </summary>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.ApiKey"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<ApiKey>> GetAllPreviewApiKeys(string spaceId = null, CancellationToken cancellationToken = default)
        {
            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/preview_api_keys", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var collection = jsonObject.ToObject<ContentfulCollection<ApiKey>>(Serializer);
            var keys = jsonObject.SelectTokens("$..items[*]").Select(c => c.ToObject<ApiKey>(Serializer));
            collection.Items = keys;

            return collection;
        }

        /// <summary>
        /// Gets an <see cref="Contentful.Core.Models.Management.ApiKey"/> in a space.
        /// </summary>
        /// <param name="apiKeyId">The id of the api key get.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.ApiKey"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ApiKey> GetApiKey(string apiKeyId, string spaceId = null, CancellationToken cancellationToken = default)
        {
            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/api_keys/{apiKeyId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
           
            return jsonObject.ToObject<ApiKey>(Serializer);
        }

        /// <summary>
        /// Gets a preview <see cref="Contentful.Core.Models.Management.ApiKey"/> in a space.
        /// </summary>
        /// <param name="apiKeyId">The id of the api key get.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.ApiKey"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ApiKey> GetPreviewApiKey(string apiKeyId, string spaceId = null, CancellationToken cancellationToken = default)
        {
            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/preview_api_keys/{apiKeyId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<ApiKey>(Serializer);
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
        public async Task<ApiKey> CreateApiKey(string name, string description, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("The name of the api key must be set.", nameof(name));
            }

            var res = await PostAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/api_keys", ConvertObjectToJsonStringContent(new { name, description }), cancellationToken, null).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<ApiKey>(Serializer);
        }

        /// <summary>
        /// Updates an <see cref="Contentful.Core.Models.Management.ApiKey"/> in a space.
        /// </summary>
        /// <param name="id">The id of the API key to update.</param>
        /// <param name="name">The name of the API key to update.</param>
        /// <param name="description">The description of the API key to update.</param>
        /// <param name="version">The last known version of the api key.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The updated <see cref="Contentful.Core.Models.Management.ApiKey"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ApiKey> UpdateApiKey(string id, string name, string description, int version, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("The name of the api key must be set.", nameof(name));
            }

            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("The id of the api key must be set.", nameof(id));
            }

            var res = await PutAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/api_keys/{id}", ConvertObjectToJsonStringContent(new { name, description }), 
                cancellationToken, version).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<ApiKey>(Serializer);
        }

        /// <summary>
        /// Deletes an api key by the specified id.
        /// </summary>
        /// <param name="apiKeyId">The id of the api key to delete.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="SystemProperties"/> with metadata of the upload.</returns>
        public async Task DeleteApiKey(string apiKeyId, string spaceId = null, CancellationToken cancellationToken = default)
        {
            var res = await DeleteAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/api_keys/{apiKeyId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a collection of all <see cref="Contentful.Core.Models.Management.User"/> in a space.
        /// </summary>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.ApiKey"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<User>> GetAllUsers(string spaceId = null, CancellationToken cancellationToken = default)
        {
            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/users", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var collection = jsonObject.ToObject<ContentfulCollection<User>>(Serializer);
            var keys = jsonObject.SelectTokens("$..items[*]").Select(c => c.ToObject<User>(Serializer));
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
        public async Task<User> GetUser(string userId, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("The id of the user must be set", nameof(userId));
            }

            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/users/{userId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<User>(Serializer);
        }

        /// <summary>
        /// Gets a single <see cref="Contentful.Core.Models.Management.User"/> for the currently logged in user.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.User"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<User> GetCurrentUser(CancellationToken cancellationToken = default)
        {
            var res = await GetAsync($"{_directApiUrl}users/me", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<User>(Serializer);
        }

        /// <summary>
        /// Gets an upload <see cref="SystemProperties"/> by the specified id.
        /// </summary>
        /// <param name="uploadId">The id of the uploaded file.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="SystemProperties"/> with metadata of the upload.</returns>
        public async Task<UploadReference> GetUpload(string uploadId, string spaceId = null, CancellationToken cancellationToken = default)
        {
            var res = await GetAsync($"{_baseUploadUrl}{spaceId ?? _options.SpaceId}/uploads/{uploadId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<UploadReference>(Serializer);
        }

        /// <summary>
        /// Uploads the specified bytes to Contentful.
        /// </summary>
        /// <param name="bytes">The bytes to upload.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="SystemProperties"/> with an id of the created upload.</returns>
        public async Task<UploadReference> UploadFile(byte[] bytes, string spaceId = null, CancellationToken cancellationToken = default)
        {
            var byteArrayContent = new ByteArrayContent(bytes);
            byteArrayContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

            var res = await PostAsync($"{_baseUploadUrl}{spaceId ?? _options.SpaceId}/uploads", byteArrayContent, cancellationToken, null).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<UploadReference>(Serializer);
        }

        /// <summary>
        /// Gets an upload <see cref="SystemProperties"/> by the specified id.
        /// </summary>
        /// <param name="uploadId">The id of the uploaded file.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="SystemProperties"/> with metadata of the upload.</returns>
        public async Task DeleteUpload(string uploadId, string spaceId = null, CancellationToken cancellationToken = default)
        {
            var res = await DeleteAsync($"{_baseUploadUrl}{spaceId ?? _options.SpaceId}/uploads/{uploadId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);
        }

        /// <summary>
        /// Uploads an array of bytes and creates an asset in Contentful as well as processing that asset.
        /// </summary>
        /// <param name="asset">The asset to create</param>
        /// <param name="bytes">The bytes to upload.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.ManagementAsset"/>.</returns>
        public async Task<ManagementAsset> UploadFileAndCreateAsset(ManagementAsset asset, byte[] bytes, string spaceId = null, CancellationToken cancellationToken = default)
        {
            var upload = await UploadFile(bytes, spaceId, cancellationToken);
            upload.SystemProperties.CreatedAt = null;
            upload.SystemProperties.CreatedBy = null;
            upload.SystemProperties.Space = null;
            upload.SystemProperties.LinkType = "Upload";
            foreach (var file in asset.Files)
            {
                file.Value.UploadReference = upload;
            }

            var createdAsset = await CreateOrUpdateAsset(asset);

            foreach (var file in createdAsset.Files) {

                await ProcessAsset(createdAsset.SystemProperties.Id, createdAsset.SystemProperties.Version ?? 1, file.Key);
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
        public async Task<ContentfulCollection<UiExtension>> GetAllExtensions(string spaceId = null, CancellationToken cancellationToken = default)
        {
            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}extensions", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var collection = jsonObject.ToObject<ContentfulCollection<UiExtension>>(Serializer);
            var keys = jsonObject.SelectTokens("$..items[*]").Select(c => c.ToObject<UiExtension>(Serializer));
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
        public async Task<UiExtension> CreateExtension(UiExtension extension, string spaceId = null, CancellationToken cancellationToken = default)
        {
            // The api does not accept a sys object in the extension creation.
            extension.SystemProperties = null;

            var res = await PostAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}extensions",
                ConvertObjectToJsonStringContent(extension), cancellationToken, null).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            
            return jsonObject.ToObject<UiExtension>(Serializer);
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
        public async Task<UiExtension> CreateOrUpdateExtension(UiExtension extension, string spaceId = null, int? version = null, CancellationToken cancellationToken = default)
        {
            if (extension.SystemProperties?.Id == null)
            {
                throw new ArgumentException("The id of the extension must be set.", nameof(extension));
            }

            var id = extension.SystemProperties?.Id;
            // The api does not accept a sys object in the extension creation.
            extension.SystemProperties = null;

            var res = await PutAsync(
                $"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}extensions/{id}",
                ConvertObjectToJsonStringContent(extension), cancellationToken, version).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var json = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return json.ToObject<UiExtension>(Serializer);
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
        public async Task<UiExtension> GetExtension(string extensionId, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(extensionId))
            {
                throw new ArgumentException("The id of the extension must be set", nameof(extensionId));
            }

            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}extensions/{extensionId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<UiExtension>(Serializer);
        }

        /// <summary>
        /// Deletes a <see cref="Contentful.Core.Models.Management.UiExtension"/> by the specified id.
        /// </summary>
        /// <param name="extensionId">The id of the extension.</param>
        /// <param name="spaceId">The id of the space to delete the extension in. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The <see name="contentTypeId">contentTypeId</see> parameter was null or empty</exception>
        public async Task DeleteExtension(string extensionId, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(extensionId))
            {
                throw new ArgumentException(nameof(extensionId));
            }

            var res = await DeleteAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/{EnvironmentsBase}extensions/{extensionId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a CMA management token that can be used to access the Contentful Management API.
        /// </summary>
        /// <param name="token">The token to create.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.ManagementToken"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ManagementToken> CreateManagementToken(ManagementToken token, CancellationToken cancellationToken = default)
        {
            var res = await PostAsync($"{_directApiUrl}users/me/access_tokens",
                ConvertObjectToJsonStringContent(new
                {
                    name = token.Name,
                    scopes = token.Scopes
                }), cancellationToken, null).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<ManagementToken>(Serializer);
        }

        /// <summary>
        /// Gets a collection of all <see cref="Contentful.Core.Models.Management.ManagementToken"/> for a user. **Note that the actual token will not be part of the response. 
        /// It is only available directly after creation of a token for security reasons.**
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.Organization"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<ManagementToken>> GetAllManagementTokens(CancellationToken cancellationToken = default)
        {
            var res = await GetAsync($"{_directApiUrl}users/me/access_tokens", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var collection = jsonObject.ToObject<ContentfulCollection<ManagementToken>>(Serializer);
            var keys = jsonObject.SelectTokens("$..items[*]").Select(c => c.ToObject<ManagementToken>(Serializer));
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
        public async Task<ManagementToken> GetManagementToken(string managementTokenId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(managementTokenId))
            {
                throw new ArgumentException("The id of the token must be set", nameof(managementTokenId));
            }

            var res = await GetAsync($"{_directApiUrl}users/me/access_tokens/{managementTokenId}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<ManagementToken>(Serializer);
        }

        /// <summary>
        /// Revokes a single <see cref="Contentful.Core.Models.Management.ManagementToken"/> for a user.
        /// </summary>
        /// <param name="managementTokenId">The id of the management token to revoke.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The revoked <see cref="Contentful.Core.Models.Management.ManagementToken"/>.</returns>
        /// <exception cref="ArgumentException">The <see name="managementTokenId">managementTokenId</see> parameter was null or empty.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ManagementToken> RevokeManagementToken(string managementTokenId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(managementTokenId))
            {
                throw new ArgumentException("The id of the token must be set", nameof(managementTokenId));
            }

            var res = await PutAsync($"{_directApiUrl}users/me/access_tokens/{managementTokenId}/revoked", null, cancellationToken, null).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<ManagementToken>(Serializer);
        }

        /// <summary>
        /// Gets a collection of all <see cref="Contentful.Core.Models.Management.Organization"/> for a user.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.Organization"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<Organization>> GetOrganizations(CancellationToken cancellationToken = default)
        {
            var res = await GetAsync($"{_directApiUrl}organizations", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var collection = jsonObject.ToObject<ContentfulCollection<Organization>>(Serializer);
            var orgs = jsonObject.SelectTokens("$..items[*]").Select(c => c.ToObject<Organization>(Serializer));
            collection.Items = orgs;

            return collection;
        }

        /// <summary>
        /// Gets a collection of all <see cref="Contentful.Core.Models.Management.ContentfulEnvironment"/> for a space.
        /// </summary>
        /// <param name="spaceId">The id of the space to get environments for. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.ContentfulEnvironment"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<ContentfulEnvironment>> GetEnvironments(string spaceId = null, CancellationToken cancellationToken = default)
        {
            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/environments", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var collection = jsonObject.ToObject<ContentfulCollection<ContentfulEnvironment>>(Serializer);
            var environments = jsonObject.SelectTokens("$..items[*]").Select(c => c.ToObject<ContentfulEnvironment>(Serializer));
            collection.Items = environments;

            return collection;
        }

        /// <summary>
        /// Creates a <see cref="Contentful.Core.Models.Management.ContentfulEnvironment"/> for a space.
        /// </summary>
        /// <param name="name">The name of the environment.</param>
        /// <param name="spaceId">The id of the space to create an environment in. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.ContentfulEnvironment"/>.</returns>
        /// <exception cref="ArgumentException">The required arguments were not provided.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulEnvironment> CreateEnvironment(string name, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("You must provide a name for the environment.", nameof(name));
            }

            var res = await PostAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/environments", ConvertObjectToJsonStringContent(new { name }), cancellationToken, null).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<ContentfulEnvironment>(Serializer);
        }

        /// <summary>
        /// Creates a <see cref="Contentful.Core.Models.Management.ContentfulEnvironment"/> for a space.
        /// </summary>
        /// <param name="id">The id of the environment to create.</param>
        /// <param name="name">The name of the environment to create.</param>
        /// <param name="version">The last known version of the environment. Must be set when updating an environment.</param>
        /// <param name="spaceId">The id of the space to create an environment in. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.ContentfulEnvironment"/>.</returns>
        /// <exception cref="ArgumentException">The required arguments were not provided.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulEnvironment> CreateOrUpdateEnvironment(string id, string name, int? version = null, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("You must provide an id for the environment.", nameof(id));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("You must provide a name for the environment.", nameof(name));
            }

            var res = await PutAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/environments/{id}", ConvertObjectToJsonStringContent(new { name }), cancellationToken, version: version).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<ContentfulEnvironment>(Serializer);
        }

        /// <summary>
        /// Clones a <see cref="Contentful.Core.Models.Management.ContentfulEnvironment"/> for a space.
        /// </summary>
        /// <param name="id">The id of the environment to create.</param>
        /// <param name="name">The name of the environment to create.</param>
        /// <param name="sourceEnvironmentId">The id of the environment to clone.</param>
        /// <param name="spaceId">The id of the space to create an environment in. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.ContentfulEnvironment"/>.</returns>
        /// <exception cref="ArgumentException">The required arguments were not provided.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulEnvironment> CloneEnvironment(string id, string name, string sourceEnvironmentId, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("You must provide an id for the environment.", nameof(id));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("You must provide a name for the environment.", nameof(name));
            }

            if (string.IsNullOrEmpty(sourceEnvironmentId))
            {
                throw new ArgumentException("You must provide an id for the source environment.", nameof(sourceEnvironmentId));
            }
            var sourceHeader = new KeyValuePair<string, IEnumerable<string>>("x-contentful-source-environment", new[] { sourceEnvironmentId });

            var res = await PutAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/environments/{id}", ConvertObjectToJsonStringContent(new { name }), cancellationToken, null, additionalHeaders: new[] { sourceHeader }.ToList()).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<ContentfulEnvironment>(Serializer);
        }

        /// <summary>
        /// Gets a <see cref="Contentful.Core.Models.Management.ContentfulEnvironment"/> for a space.
        /// </summary>
        /// <param name="id">The id of the environment to get.</param>
        /// <param name="spaceId">The id of the space to get an environment in. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.ContentfulEnvironment"/>.</returns>
        /// <exception cref="ArgumentException">The required arguments were not provided.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulEnvironment> GetEnvironment(string id, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("You must provide an id for the environment.", nameof(id));
            }

            var res = await GetAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/environments/{id}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<ContentfulEnvironment>(Serializer);
        }

        /// <summary>
        /// Deletes a <see cref="Contentful.Core.Models.Management.ContentfulEnvironment"/> for a space.
        /// </summary>
        /// <param name="id">The id of the environment to delete.</param>
        /// <param name="spaceId">The id of the space to get an environment in. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <exception cref="ArgumentException">The required arguments were not provided.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task DeleteEnvironment(string id, string spaceId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("You must provide an id for the environment.", nameof(id));
            }

            var res = await DeleteAsync($"{_baseUrl}{spaceId ?? _options.SpaceId}/environments/{id}", cancellationToken).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a collection of all <see cref="Contentful.Core.Models.Management.UsagePeriod"/> for an organization.
        /// </summary>
        /// <param name="organizationId">The id of the organization to get usage periods for.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.UsagePeriod"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<UsagePeriod>> GetUsagePeriods(string organizationId, CancellationToken cancellationToken = default)
        {
            var alphaHeader = new KeyValuePair<string, IEnumerable<string>>("x-contentful-enable-alpha-feature", new[] { "usage-insights" });
            var res = await GetAsync($"{_directApiUrl}organizations/{organizationId}/usage_periods", cancellationToken, additionalHeaders: new[] { alphaHeader }.ToList()).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            var collection = jsonObject.ToObject<ContentfulCollection<UsagePeriod>>(Serializer);
            var periods = jsonObject.SelectTokens("$..items[*]").Select(c => c.ToObject<UsagePeriod>(Serializer));
            collection.Items = periods;
            return collection;
        }

        /// <summary>
        /// Gets a collection of <see cref="Contentful.Core.Models.Management.OrganizationMembership"/> for the specified organization.
        /// </summary>
        /// <param name="organizationId">The id of the organization.</param>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A collection of <see cref="Contentful.Core.Models.Management.OrganizationMembership"/>.</returns>
        public async Task<ContentfulCollection<OrganizationMembership>> GetOrganizationMemberships(string organizationId, string queryString = null, CancellationToken cancellationToken = default)
        {
            var alphaHeader = new KeyValuePair<string, IEnumerable<string>>("x-contentful-enable-alpha-feature", new[] { "organization-user-management-api" });
            var res = await GetAsync($"{_directApiUrl}organizations/{organizationId}/organization_memberships{queryString}", cancellationToken, additionalHeaders: new[] { alphaHeader }.ToList()).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            var collection = jsonObject.ToObject<ContentfulCollection<OrganizationMembership>>(Serializer);
            var memberships = jsonObject.SelectTokens("$..items[*]").Select(c => c.ToObject<OrganizationMembership>(Serializer));
            collection.Items = memberships;

            return collection;
        }

        /// <summary>
        /// Gets a collection of <see cref="Contentful.Core.Models.Management.ApiUsage"/> for an organization.
        /// </summary>
        /// <param name="organizationId">The id of the organization to get usage for.</param>
        /// <param name="type">The type of resource to get usage for, organization or space.</param>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.ApiUsage"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<ApiUsage>> GetResourceUsage(string organizationId, string type, string queryString = null, CancellationToken cancellationToken = default)
        {
            var alphaHeader = new KeyValuePair<string, IEnumerable<string>>("x-contentful-enable-alpha-feature", new[] { "usage-insights" });

            var res = await GetAsync($"{_directApiUrl}organizations/{organizationId}/usages/{type}{queryString}", cancellationToken, additionalHeaders: new[] { alphaHeader }.ToList()).ConfigureAwait(false);
            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var collection = jsonObject.ToObject<ContentfulCollection<ApiUsage>>(Serializer);
            var apiUsage = jsonObject.SelectTokens("$..items[*]").Select(c => c.ToObject<ApiUsage>(Serializer));
            collection.Items = apiUsage;

            return collection;
        }

        /// <summary>
        /// Creates a membership in an <see cref="Organization"/>.
        /// </summary>
        /// <param name="organizationId">The id of the organization to create a membership in.</param>
        /// <param name="role">The role the membership should have for that organization.</param>
        /// <param name="email">The email address of the membership.</param>
        /// <param name="suppressInvitation">Whether or not to suppress the invitation email.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.OrganizationMembership"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<OrganizationMembership> CreateOrganizationMembership(string organizationId, string role, string email, bool suppressInvitation, CancellationToken cancellationToken = default)
        {
            var alphaHeader = new KeyValuePair<string, IEnumerable<string>>("x-contentful-enable-alpha-feature", new[] { "organization-user-management-api" });

            var res = await PostAsync($"{_directApiUrl}organizations/{organizationId}/organization_memberships", ConvertObjectToJsonStringContent(new { role, email, suppressInvitation }), cancellationToken, null, additionalHeaders: new[] { alphaHeader }.ToList()).ConfigureAwait(false);
            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var collection = jsonObject.ToObject<ContentfulCollection<ApiUsage>>(Serializer);
            var apiUsage = jsonObject.SelectTokens("$..items[*]").Select(c => c.ToObject<ApiUsage>(Serializer));
            collection.Items = apiUsage;
            return jsonObject.ToObject<OrganizationMembership>(Serializer);
        }
        
        /// <summary>
        /// Gets a single <see cref="Contentful.Core.Models.Management.OrganizationMembership"/> for a space.
        /// </summary>
        /// <param name="membershipId">The id of the membership to get.</param>
        /// <param name="organizationId">The id of the organization.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.SpaceMembership"/>.</returns>
        /// <exception cref="ArgumentException">The <see name="spaceMembershipId">spaceMembershipId</see> parameter was null or empty.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<OrganizationMembership> GetOrganizationMembership(string membershipId, string organizationId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(membershipId))
            {
                throw new ArgumentException("The id of the organization membership must be set", nameof(membershipId));
            }
            var alphaHeader = new KeyValuePair<string, IEnumerable<string>>("x-contentful-enable-alpha-feature", new[] { "organization-user-management-api" });

            var res = await GetAsync($"{_directApiUrl}organizations/{organizationId}/organization_memberships/{membershipId}", cancellationToken, additionalHeaders: new[] { alphaHeader }.ToList()).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<OrganizationMembership>(Serializer);
        }

        /// <summary>
        /// Updates a <see cref="Contentful.Core.Models.Management.OrganizationMembership"/> for a space.
        /// </summary>
        /// <param name="role">The role to set for the membership.</param>
        /// <param name="membershipId">The id of the membership to update.</param>
        /// <param name="organizationId">The id of the organization.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.OrganizationMembership"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<OrganizationMembership> UpdateOrganizationMembership(string role, string membershipId, string organizationId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(membershipId))
            {
                throw new ArgumentException("The id of the organization membership must be set.", nameof(membershipId));
            }
            var alphaHeader = new KeyValuePair<string, IEnumerable<string>>("x-contentful-enable-alpha-feature", new[] { "organization-user-management-api" });

            var res = await PutAsync($"{_directApiUrl}organizations/{organizationId}/organization_memberships/{membershipId}",
                ConvertObjectToJsonStringContent(new { role }), cancellationToken, null, additionalHeaders: new[] { alphaHeader }.ToList()).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return jsonObject.ToObject<OrganizationMembership>(Serializer);
        }

        /// <summary>
        /// Deletes a <see cref="Contentful.Core.Models.Management.OrganizationMembership"/> for a space.
        /// </summary>
        /// <param name="membershipId">The id of the organization membership to delete.</param>
        /// <param name="organizationId">The id of the organization.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <exception cref="ArgumentException">The <see name="membershipId">membershipId</see> parameter was null or empty.</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task DeleteOrganizationMembership(string membershipId, string organizationId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(membershipId))
            {
                throw new ArgumentException("The id of the space membership must be set", nameof(membershipId));
            }
            var alphaHeader = new KeyValuePair<string, IEnumerable<string>>("x-contentful-enable-alpha-feature", new[] { "organization-user-management-api" });

            var res = await DeleteAsync($"{_directApiUrl}organizations/{organizationId}/organization_memberships/{membershipId}", cancellationToken, additionalHeaders: new[] { alphaHeader }.ToList()).ConfigureAwait(false);

            await EnsureSuccessfulResult(res).ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> PostAsync(string url, HttpContent content, CancellationToken cancellationToken, int? version, string contentTypeId = null, string organisationId = null, List<KeyValuePair<string, IEnumerable<string>>> additionalHeaders = null)
        {
            return await SendHttpRequest(url, HttpMethod.Post, _options.ManagementApiKey, cancellationToken, content, version, contentTypeId, organisationId, additionalHeaders: additionalHeaders).ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> PutAsync(string url, HttpContent content, CancellationToken cancellationToken, int? version, string contentTypeId = null, string organisationId = null, List<KeyValuePair<string, IEnumerable<string>>> additionalHeaders = null)
        {
            return await SendHttpRequest(url, HttpMethod.Put, _options.ManagementApiKey, cancellationToken, content, version, contentTypeId, organisationId, additionalHeaders: additionalHeaders).ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> DeleteAsync(string url, CancellationToken cancellationToken, int? version = null, List<KeyValuePair<string, IEnumerable<string>>> additionalHeaders = null)
        {
            return await SendHttpRequest(url, HttpMethod.Delete, _options.ManagementApiKey, cancellationToken, version: version, additionalHeaders: additionalHeaders).ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> GetAsync(string url, CancellationToken cancellationToken, int? version = null, List<KeyValuePair<string, IEnumerable<string>>> additionalHeaders = null)
        {
            return await SendHttpRequest(url, HttpMethod.Get, _options.ManagementApiKey, cancellationToken, version: version, additionalHeaders: additionalHeaders).ConfigureAwait(false);
        }

        private string ConvertObjectToJsonString(object ob)
        {
            var resolver = new CamelCasePropertyNamesContractResolver();
            resolver.NamingStrategy.OverrideSpecifiedNames = false;

            var settings = new JsonSerializerSettings
            {
                ContractResolver = resolver,
            };

            settings.Converters.Add(new ExtensionJsonConverter());

            var serializedObject = JsonConvert.SerializeObject(ob, settings);

            return serializedObject;
        }

        private StringContent ConvertObjectToJsonStringContent(object ob)
        {
            var serializedObject = ConvertObjectToJsonString(ob);
            return new StringContent(serializedObject, Encoding.UTF8, "application/vnd.contentful.management.v1+json");
        }
    }
}
