using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Contentful.Core.Configuration;
using Contentful.Core.Errors;
using Contentful.Core.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Text;
using Contentful.Core.Search;

namespace Contentful.Core
{
    /// <summary>
    /// Main class for interaction with the contentful delivery and preview APIs
    /// </summary>
    public class ContentfulClient : IContentfulClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://cdn.contentful.com/spaces/";
        private readonly ContentfulOptions _options;


        /// <summary>
        /// Initializes a new instance of the <see cref="ContentfulClient"/> class. 
        /// The main class for interaction with the contentful deliver and preview APIs.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> of your application.</param>
        /// <param name="options">The options object used to retrieve the <see cref="ContentfulOptions"/> for this client.</param>
        public ContentfulClient(HttpClient httpClient, IOptions<ContentfulOptions> options)
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

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_options.DeliveryApiKey}");

            if (_options.UsePreviewApi)
            {
                _baseUrl = _baseUrl.Replace("cdn", "preview");
            }
        }

        /// <summary>
        /// Returns whether or not the client is using the preview API.
        /// </summary>
        public bool IsPreviewClient => _options?.UsePreviewApi ?? false;

        /// <summary>
        /// Get a single entry by the specified ID.
        /// </summary>
        /// <typeparam name="T">The type to serialize this entry into. If you want the metadata to 
        /// be included in the serialized response use the <see cref="Entry{T}"/> class as a type parameter.</typeparam>
        /// <param name="entryId">The ID of the entry.</param>
        /// <returns>The response from the API serialized into <see cref="T"/></returns>
        /// <exception cref="ContentfulException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public async Task<T> GetEntryAsync<T>(string entryId)
        {
            if (string.IsNullOrEmpty(entryId))
            {
                throw new ArgumentException(nameof(entryId));
            }

            var res = await _httpClient.GetAsync($"{_baseUrl}{_options.SpaceId}/entries/{entryId}");

            if (!res.IsSuccessStatusCode)
            {
                await CreateExceptionForFailedRequest(res);
            }

            var ob = default(T);

            if (typeof(IContentfulResource).GetTypeInfo().IsAssignableFrom(typeof(T)))
            {
                ob = JObject.Parse(await res.Content.ReadAsStringAsync()).ToObject<T>();
            }
            else
            {
                ob = JObject.Parse(await res.Content.ReadAsStringAsync()).SelectToken("$.fields").ToObject<T>();
            }
            return ob;
        }

        /// <summary>
        /// Gets all the entries with the specified content type.
        /// </summary>
        /// <typeparam name="T">The class to serialize the response into. If you want the metadata to 
        /// be included in the serialized response use the <see cref="Entry{T}"/> class as a type parameter.</typeparam>
        /// <param name="contentTypeId">The ID of the content type to get entries for.</param>
        /// <param name="queryBuilder">The optional <see cref="QueryBuilder"/> to add additional filtering to the query.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of objects seralized from the API response.</returns>
        /// <exception cref="ContentfulException"></exception>
        public async Task<IEnumerable<T>> GetEntriesByType<T>(string contentTypeId, QueryBuilder queryBuilder = null)
        {
            var builder = queryBuilder ?? new QueryBuilder();

            builder.ContentTypeIs(contentTypeId);

            return await GetEntriesAsync<T>(builder);
        }

        /// <summary>
        /// Gets all the entries of a space, filtered by an optional <see cref="QueryBuilder"/>.
        /// </summary>
        /// <typeparam name="T">The class to serialize the response into. If you want the metadata to 
        /// be included in the serialized response use the <see cref="Entry{T}"/> class as a type parameter.</typeparam>
        /// <param name="queryBuilder">The optional <see cref="QueryBuilder"/> to add additional filtering to the query.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of objects seralized from the API response.</returns>
        /// <exception cref="ContentfulException"></exception>
        public async Task<IEnumerable<T>> GetEntriesAsync<T>(QueryBuilder queryBuilder)
        {
            return await GetEntriesAsync<T>(queryBuilder?.Build());
        }

        /// <summary>
        /// Gets all the entries of a space, filtered by an optional querystring. A simpler approach than 
        /// to construct a query manually is to use the <see cref="QueryBuilder"/> class.
        /// </summary>
        /// <typeparam name="T">The class to serialize the response into. If you want the metadata to 
        /// be included in the serialized response use the <see cref="Entry{T}"/> class as a type parameter.</typeparam>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of objects seralized from the API response.</returns>
        /// <exception cref="ContentfulException"></exception>
        public async Task<IEnumerable<T>> GetEntriesAsync<T>(string queryString = null)
        {
            var res = await _httpClient.GetAsync($"{_baseUrl}{_options.SpaceId}/entries{queryString}");

            if (!res.IsSuccessStatusCode)
            {
                await CreateExceptionForFailedRequest(res);
            }

            IEnumerable<T> ob;
            if (typeof(IContentfulResource).GetTypeInfo().IsAssignableFrom(typeof(T)))
            {
                ob = JObject.Parse(await res.Content.ReadAsStringAsync()).SelectTokens("$..items[*]")
                        .Select(t => t.ToObject<T>()); ;
            }
            else
            {
                ob =
                   JObject.Parse(await res.Content.ReadAsStringAsync())
                       .SelectTokens("$..items..fields")
                       .Select(t => t.ToObject<T>());
            }
            return ob;
        }

        /// <summary>
        /// Gets all the entries of a space, filtered by an optional <see cref="QueryBuilder"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="IContentfulResource"/> to serialize the response into.</typeparam>
        /// <param name="queryBuilder">The optional <see cref="QueryBuilder"/> to add additional filtering to the query.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of items.</returns>
        /// <exception cref="ContentfulException"></exception>
        public async Task<ContentfulCollection<T>> GetEntriesCollectionAsync<T>(QueryBuilder queryBuilder) where T : IContentfulResource
        {
            return await GetEntriesCollectionAsync<T>(queryBuilder.Build());
        }

        /// <summary>
        /// Gets all the entries of a space, filtered by an optional querystring. A simpler approach than 
        /// to construct a query manually is to use the <see cref="QueryBuilder"/> class.
        /// </summary>
        /// <typeparam name="T">The <see cref="IContentfulResource"/> to serialize the response into.</typeparam>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of items.</returns>
        /// <exception cref="ContentfulException"></exception>
        public async Task<ContentfulCollection<T>> GetEntriesCollectionAsync<T>(string queryString = null) where T : IContentfulResource
        {
            var res = await _httpClient.GetAsync($"{_baseUrl}{_options.SpaceId}/entries{queryString}");

            if (!res.IsSuccessStatusCode)
            {
                await CreateExceptionForFailedRequest(res);
            }

            return JObject.Parse(await res.Content.ReadAsStringAsync()).ToObject<ContentfulCollection<T>>();
        }

        /// <summary>
        /// Gets a single <see cref="Asset"/> by the specified ID.
        /// </summary>
        /// <param name="assetId">The ID of the asset.</param>
        /// <returns>The response from the API serialized into an <see cref="Asset"/></returns>
        /// <exception cref="ContentfulException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public async Task<Asset> GetAssetAsync(string assetId)
        {
            if (string.IsNullOrEmpty(assetId))
            {
                throw new ArgumentException(nameof(assetId));
            }

            var res = await _httpClient.GetAsync($"{_baseUrl}{_options.SpaceId}/assets/{assetId}");

            if (!res.IsSuccessStatusCode)
            {
                await CreateExceptionForFailedRequest(res);
            }

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync());
            var asset = jsonObject.ToObject<Asset>();

            asset.Title = jsonObject.SelectToken("$.fields.title").ToString();
            asset.Description = jsonObject.SelectToken("$.fields.description").ToString();
            asset.File = jsonObject.SelectToken("$.fields.file").ToObject<File>();

            return asset;
        }

        /// <summary>
        /// Gets all assets of a space, filtered by an optional <see cref="QueryBuilder"/>.
        /// </summary>
        /// <param name="queryBuilder">The optional <see cref="QueryBuilder"/> to add additional filtering to the query.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Asset"/>.</returns>
        /// <exception cref="ContentfulException"></exception>
        public async Task<IEnumerable<Asset>> GetAssetsAsync(QueryBuilder queryBuilder)
        {
            return await GetAssetsAsync(queryBuilder.Build());
        }

        /// <summary>
        /// Gets all assets of a space, filtered by an optional querystring. A simpler approach than 
        /// to construct a query manually is to use the <see cref="QueryBuilder"/> class.
        /// </summary>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Asset"/>.</returns>
        /// <exception cref="ContentfulException"></exception>
        public async Task<IEnumerable<Asset>> GetAssetsAsync(string queryString = null)
        {
            var res = await _httpClient.GetAsync($"{_baseUrl}{_options.SpaceId}/assets/{queryString}");

            if (!res.IsSuccessStatusCode)
            {
                await CreateExceptionForFailedRequest(res);
            }

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync());
            var asset = jsonObject.SelectTokens("$..items[*]").Select(c => new Asset()
            {
                Title = c.SelectToken("$.fields.title").ToString(),
                Description = c.SelectToken("$.fields.description").ToString(),
                File = c.SelectToken("$.fields.file").ToObject<File>()
            });

            return asset;
        }

        /// <summary>
        /// Gets all assets of a space, filtered by an optional <see cref="QueryBuilder"/>.
        /// </summary>
        /// <param name="queryBuilder">The optional <see cref="QueryBuilder"/> to add additional filtering to the query.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Asset"/>.</returns>
        /// <exception cref="ContentfulException"></exception>
        public async Task<ContentfulCollection<Asset>> GetAssetsCollectionAsync(QueryBuilder queryBuilder)
        {
            return await GetAssetsCollectionAsync(queryBuilder.Build());
        }

        /// <summary>
        /// Gets all assets of a space, filtered by an optional queryString. A simpler approach than 
        /// to construct a query manually is to use the <see cref="QueryBuilder"/> class.
        /// </summary>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Asset"/>.</returns>
        /// <exception cref="ContentfulException"></exception>
        public async Task<ContentfulCollection<Asset>> GetAssetsCollectionAsync(string queryString = null)
        {
            var res = await _httpClient.GetAsync($"{_baseUrl}{_options.SpaceId}/assets/{queryString}");

            if (!res.IsSuccessStatusCode)
            {
                await CreateExceptionForFailedRequest(res);
            }

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync());
            var collection = jsonObject.ToObject<ContentfulCollection<Asset>>();
            var assets = jsonObject.SelectTokens("$..items[*]").Select(c => new Asset()
            {
                Title = c.SelectToken("$.fields.title").ToString(),
                Description = c.SelectToken("$.fields.description").ToString(),
                File = c.SelectToken("$.fields.file").ToObject<File>()
            });
            collection.Items = assets;
            return collection;
        }

        /// <summary>
        /// Gets the <see cref="Space"/> for this client.
        /// </summary>
        /// <returns>The <see cref="Space"/>.</returns>
        /// <exception cref="ContentfulException"></exception>
        public async Task<Space> GetSpaceAsync()
        {
            var res = await _httpClient.GetAsync($"{_baseUrl}{_options.SpaceId}");

            if (!res.IsSuccessStatusCode)
            {
                await CreateExceptionForFailedRequest(res);
            }

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync());
            var space = jsonObject.ToObject<Space>();

            return space;
        }

        /// <summary>
        /// Gets a <see cref="ContentType"/> by the specified ID.
        /// </summary>
        /// <param name="contentTypeId">The ID of the content type.</param>
        /// <returns>The response from the API serialized into a <see cref="ContentType"/>.</returns>
        /// <exception cref="ContentfulException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public async Task<ContentType> GetContentTypeAsync(string contentTypeId)
        {
            if (string.IsNullOrEmpty(contentTypeId))
            {
                throw new ArgumentException(nameof(contentTypeId));
            }

            var res = await _httpClient.GetAsync($"{_baseUrl}{_options.SpaceId}/content_types/{contentTypeId}");

            if (!res.IsSuccessStatusCode)
            {
                await CreateExceptionForFailedRequest(res);
            }

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync());
            var contentType = jsonObject.ToObject<ContentType>();

            return contentType;
        }

        /// <summary>
        /// Get all content types of a space.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ContentType"/>.</returns>
        public async Task<IEnumerable<ContentType>> GetContentTypesAsync()
        {
            var res = await _httpClient.GetAsync($"{_baseUrl}{_options.SpaceId}/content_types/");

            if (!res.IsSuccessStatusCode)
            {
                await CreateExceptionForFailedRequest(res);
            }

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync());
            var contentTypes = jsonObject.SelectTokens("$..items[*]").Select(t => t.ToObject<ContentType>());

            return contentTypes;
        }

        /// <summary>
        /// Fetches a sync result of content. 
        /// </summary>
        /// <param name="syncType">The optional type of items that should be synced.</param>
        /// <param name="contentTypeId">The content type ID to filter entries by. Only applicable when the syncType is <see cref="SyncType.Entry"/>.</param>
        /// <param name="initial">Whether this is an initial sync or not.</param>
        /// <returns>A <see cref="SyncResult"/> containing all synced resources.</returns>
        public async Task<SyncResult> Sync(SyncType syncType = SyncType.All, string contentTypeId = "", bool initial = false)
        {
            var query = BuildSyncQuery(syncType, contentTypeId, initial);

            var res = await _httpClient.GetAsync($"{_baseUrl}{_options.SpaceId}/sync{query}");

            if (!res.IsSuccessStatusCode)
            {
                await CreateExceptionForFailedRequest(res);
            }

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync());
            var syncResult = jsonObject.ToObject<SyncResult>();
            var entries =
                jsonObject.SelectTokens("$.items[?(@.sys.type=='Entry')]").Select(c => c.ToObject<Entry<dynamic>>());
            var assets =
                jsonObject.SelectTokens("$.items[?(@.sys.type=='Asset')]").Select(c => c.ToObject<SyncedAsset>());
            var deletedEntries = jsonObject.SelectTokens("$.items[?(@.sys.type=='DeletedEntry')].sys").Select(c => c.ToObject<SystemProperties>());
            var deletedAssets = jsonObject.SelectTokens("$.items[?(@.sys.type=='DeletedAsset')].sys").Select(c => c.ToObject<SystemProperties>());

            syncResult.Assets = assets;
            syncResult.DeletedAssets = deletedAssets;
            syncResult.DeletedEntries = deletedEntries;
            syncResult.Entries = entries;

            return syncResult;
        }

        private string BuildSyncQuery(SyncType syncType, string contentTypeId, bool initial)
        {
            var querystringValues = new List<KeyValuePair<string, string>>();

            if (syncType != SyncType.All)
            {
                querystringValues.Add(new KeyValuePair<string, string>("type", syncType.ToString()));
            }

            if (syncType == SyncType.Entry && !string.IsNullOrEmpty(contentTypeId))
            {
                querystringValues.Add(new KeyValuePair<string, string>("content_type", contentTypeId));
            }

            if (initial)
            {
                querystringValues.Add(new KeyValuePair<string, string>("initial", "true"));
            }

            var query = new StringBuilder();

            var hasQuery = false;

            foreach (var parameter in querystringValues)
            {
                query.Append(hasQuery ? '&' : '?');
                query.Append(parameter.Key);
                query.Append('=');
                query.Append(parameter.Value);
                hasQuery = true;
            }

            return query.ToString();
        }


        private async Task CreateExceptionForFailedRequest(HttpResponseMessage res)
        {
            var jsonError = JObject.Parse(await res.Content.ReadAsStringAsync());
            var sys = jsonError.SelectToken("$.sys").ToObject<SystemProperties>();
            var errorDetails = jsonError.SelectToken("$.details")?.ToObject<ErrorDetails>();
            var ex = new ContentfulException((int)res.StatusCode, jsonError.SelectToken("$.message").ToString());
            ex.RequestId = jsonError.SelectToken("$.requestId").ToString();
            ex.ErrorDetails = errorDetails;
            ex.SystemProperties = sys;

            throw ex;
        }
    }
}
