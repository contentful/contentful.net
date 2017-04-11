﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Text;
using Contentful.Core.Search;
using System.Threading;
using Contentful.Core.Errors;

namespace Contentful.Core
{
    /// <summary>
    /// Main class for interaction with the contentful delivery and preview APIs
    /// </summary>
    public class ContentfulClient : ContentfulClientBase, IContentfulClient
    {
        private readonly string _baseUrl = "https://cdn.contentful.com/spaces/";

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentfulClient"/> class. 
        /// The main class for interaction with the contentful deliver and preview APIs.
        /// </summary>
        /// <param name="httpClient">The HttpClient of your application.</param>
        /// <param name="options">The options object used to retrieve the <see cref="ContentfulOptions"/> for this client.</param>
        /// <exception cref="ArgumentException">The <see name="options">options</see> parameter was null or empty</exception>
        public ContentfulClient(HttpClient httpClient, IOptions<ContentfulOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;

            if (options == null)
            {
                throw new ArgumentException("The ContentfulOptions cannot be null.", nameof(options));
            }

            if (_options.UsePreviewApi)
            {
                _baseUrl = _baseUrl.Replace("cdn", "preview");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentfulClient"/> class.
        /// </summary>
        /// <param name="httpClient">The HttpClient of your application.</param>
        /// <param name="options">The <see cref="ContentfulOptions"/> used for this client.</param>
        public ContentfulClient(HttpClient httpClient, ContentfulOptions options):
            this(httpClient, new OptionsWrapper<ContentfulOptions>(options))
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentfulClient"/> class.
        /// </summary>
        /// <param name="httpClient">The HttpClient of your application.</param>
        /// <param name="deliveryApiKey">The delivery API key used when communicating with the Contentful API</param>
        /// <param name="spaceId">The ID of the space to fetch content from.</param>
        /// <param name="usePreviewApi">Whether or not to use the Preview API for requests.
        /// If this is set to true the preview API key needs to be used for <paramref name="deliveryApiKey"/>
        ///  </param>
        public ContentfulClient(HttpClient httpClient, string deliveryApiKey, string spaceId, bool usePreviewApi = false):
            this(httpClient, new OptionsWrapper<ContentfulOptions>(new ContentfulOptions()
            {
                DeliveryApiKey = deliveryApiKey,
                SpaceId = spaceId,
                UsePreviewApi = usePreviewApi
            }))
        {

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
        /// <param name="queryBuilder">The optional <see cref="QueryBuilder{T}"/> to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into <typeparamref name="T"/></returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The entryId parameter was null or empty.</exception>
        public async Task<T> GetEntryAsync<T>(string entryId, QueryBuilder<T> queryBuilder, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetEntryAsync<T>(entryId, queryBuilder?.Build(), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Get a single entry by the specified ID.
        /// </summary>
        /// <typeparam name="T">The type to serialize this entry into. If you want the metadata to 
        /// be included in the serialized response use the <see cref="Entry{T}"/> class as a type parameter.</typeparam>
        /// <param name="entryId">The ID of the entry.</param>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into <typeparamref name="T"/></returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The entryId parameter was null or empty.</exception>
        public async Task<T> GetEntryAsync<T>(string entryId, string queryString = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(entryId))
            {
                throw new ArgumentException(nameof(entryId));
            }

            var res = await GetAsync($"{_baseUrl}{_options.SpaceId}/entries/{entryId}{queryString}", cancellationToken).ConfigureAwait(false);

            var ob = default(T);

            if (typeof(IContentfulResource).GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()))
            {
                ob = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false)).ToObject<T>();
            }
            else
            {
                var json = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

                //move the sys object beneath the fields to make serialization more logical for the end user.
                var sys = json.SelectToken("$.sys");
                var fields = json.SelectToken("$.fields");
                fields["sys"] = sys;
                ob = fields.ToObject<T>();
            }
            return ob;
        }

        /// <summary>
        /// Gets all the entries with the specified content type.
        /// </summary>
        /// <typeparam name="T">The class to serialize the response into. If you want the metadata to 
        /// be included in the serialized response use the <see cref="Entry{T}"/> class as a type parameter.</typeparam>
        /// <param name="contentTypeId">The ID of the content type to get entries for.</param>
        /// <param name="queryBuilder">The optional <see cref="QueryBuilder{T}"/> to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of objects seralized from the API response.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<IEnumerable<T>> GetEntriesByTypeAsync<T>(string contentTypeId, QueryBuilder<T> queryBuilder = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var builder = queryBuilder ?? new QueryBuilder<T>();

            builder.ContentTypeIs(contentTypeId);

            return await GetEntriesAsync<T>(builder, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all the entries of a space, filtered by an optional <see cref="QueryBuilder{T}"/>.
        /// </summary>
        /// <typeparam name="T">The class to serialize the response into. If you want the metadata to 
        /// be included in the serialized response use the <see cref="Entry{T}"/> class as a type parameter.</typeparam>
        /// <param name="queryBuilder">The optional <see cref="QueryBuilder{T}"/> to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of objects seralized from the API response.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<IEnumerable<T>> GetEntriesAsync<T>(QueryBuilder<T> queryBuilder, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetEntriesAsync<T>(queryBuilder?.Build(), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all the entries of a space, filtered by an optional querystring. A simpler approach than 
        /// to construct a query manually is to use the <see cref="QueryBuilder{T}"/> class.
        /// </summary>
        /// <typeparam name="T">The class to serialize the response into. If you want the metadata to 
        /// be included in the serialized response use the <see cref="Entry{T}"/> class as a type parameter.</typeparam>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of objects seralized from the API response.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<IEnumerable<T>> GetEntriesAsync<T>(string queryString = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var res = await GetAsync($"{_baseUrl}{_options.SpaceId}/entries{queryString}", cancellationToken).ConfigureAwait(false);

            IEnumerable<T> entries;

            var json = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            HashSet<string> processedIds = new HashSet<string>();
            foreach (var item in json.SelectTokens("$.items[*]").OfType<JObject>())
            {
                ResolveLinks(json, item, processedIds);
            }

            if (typeof(IContentfulResource).GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()))
            {
                entries = json.SelectTokens("$.items[*]")
                    .Select(t => t.ToObject<T>());
            }
            else
            {
                var entryTokens = json.SelectTokens("$.items[*].fields");

                //Move sys properties into the fields object to make serialization more logical on client
                foreach (var token in entryTokens)
                {
                    var sys = token.Parent.Parent["sys"];
                    token["sys"] = sys;
                }
                entries = entryTokens.Select(t => t.ToObject<T>());
            }
            return entries;
        }

        private void ResolveLinks(JObject json, JObject entryToken, ISet<string> processedIds)
        {
            var id = ((JValue) entryToken.SelectToken("$.sys.id")).Value.ToString();
            entryToken.Add("$id", new JValue(id));
            processedIds.Add(id);
            var links = entryToken.SelectTokens("$.fields..sys").ToList();

            //Walk through and add any included entries as direct links.
            for (var i = links.Count - 1; i >= 0; i--)
            {
                var linkToken = links[i];
                var linkId = ((JValue)linkToken["id"]).Value.ToString();
                JToken replacementToken = null;
                if (processedIds.Contains(linkId))
                {
                    replacementToken = new JObject();
                    replacementToken["$ref"] = linkId;
                }
                else if (!string.IsNullOrEmpty(linkToken["linkType"]?.ToString()))
                {
                    replacementToken = json.SelectTokens($"$.includes.{linkToken["linkType"]}[?(@.sys.id=='{linkToken["id"]}')]").FirstOrDefault();

                    if (replacementToken == null)
                    {
                        //This could be due to the referenced entry being part of the original request (circular reference), so scan through that as well.
                        replacementToken = json.SelectTokens($"$.items.[?(@.sys.id=='{linkToken["id"]}')]").FirstOrDefault();
                    }
                }
                if (replacementToken != null)
                {
                    var grandParent = (JObject)linkToken.Parent.Parent;
                    grandParent.RemoveAll();
                    grandParent.Add(replacementToken.Children());

                    if (!processedIds.Contains(linkId))
                    {
                        ResolveLinks(json, grandParent, processedIds);
                    }
                }
            }
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

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var collection = jsonObject.ToObject<ContentfulCollection<T>>();


            collection.IncludedAssets = jsonObject.SelectTokens("$.includes.Asset[*]")?.Select(t => t.ToObject<Asset>());

            collection.IncludedEntries = jsonObject.SelectTokens("$.includes.Entry[*]")?.Select(t => t.ToObject<Entry<dynamic>>());

            return collection;
        }

        /// <summary>
        /// Gets a single <see cref="Asset"/> by the specified ID.
        /// </summary>
        /// <param name="assetId">The ID of the asset.</param>
        /// <param name="queryBuilder">The optional <see cref="QueryBuilder{T}"/> to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into an <see cref="Asset"/></returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The <see name="assetId">assetId</see> parameter was null or emtpy.</exception>
        public async Task<Asset> GetAssetAsync(string assetId, QueryBuilder<Asset> queryBuilder, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetAssetAsync(assetId, queryBuilder?.Build(), cancellationToken);
        }

        /// <summary>
        /// Gets a single <see cref="Asset"/> by the specified ID.
        /// </summary>
        /// <param name="assetId">The ID of the asset.</param>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into an <see cref="Asset"/></returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The <see name="assetId">assetId</see> parameter was null or emtpy.</exception>
        public async Task<Asset> GetAssetAsync(string assetId, string queryString = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(assetId))
            {
                throw new ArgumentException(nameof(assetId));
            }

            var res = await GetAsync($"{_baseUrl}{_options.SpaceId}/assets/{assetId}{queryString}", cancellationToken).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var asset = jsonObject.ToObject<Asset>();

            return asset;
        }

        /// <summary>
        /// Gets all assets of a space, filtered by an optional <see cref="QueryBuilder{T}"/>.
        /// </summary>
        /// <param name="queryBuilder">The optional <see cref="QueryBuilder{T}"/> to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Asset"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<IEnumerable<Asset>> GetAssetsAsync(QueryBuilder<Asset> queryBuilder, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetAssetsAsync(queryBuilder?.Build(), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all assets of a space, filtered by an optional querystring. A simpler approach than 
        /// to construct a query manually is to use the <see cref="QueryBuilder{T}"/> class.
        /// </summary>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Asset"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<IEnumerable<Asset>> GetAssetsAsync(string queryString = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var res = await GetAsync($"{_baseUrl}{_options.SpaceId}/assets/{queryString}", cancellationToken).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var asset = jsonObject.SelectTokens("$..items[*]").Select(c => c.ToObject<Asset>());

            return asset;
        }

        /// <summary>
        /// Gets all assets of a space, filtered by an optional <see cref="QueryBuilder{T}"/>.
        /// </summary>
        /// <param name="queryBuilder">The optional <see cref="QueryBuilder{T}"/> to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Asset"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<Asset>> GetAssetsCollectionAsync(QueryBuilder<Asset> queryBuilder, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetAssetsCollectionAsync(queryBuilder?.Build(), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all assets of a space, filtered by an optional queryString. A simpler approach than 
        /// to construct a query manually is to use the <see cref="QueryBuilder{T}"/> class.
        /// </summary>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Asset"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<Asset>> GetAssetsCollectionAsync(string queryString = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var res = await GetAsync($"{_baseUrl}{_options.SpaceId}/assets/{queryString}", cancellationToken).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var collection = jsonObject.ToObject<ContentfulCollection<Asset>>();
            var assets = jsonObject.SelectTokens("$.items[*]").Select(c => c.ToObject<Asset>()); ;
            collection.Items = assets;
            return collection;
        }

        /// <summary>
        /// Gets the <see cref="Space"/> for this client.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Space"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<Space> GetSpaceAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var res = await GetAsync($"{_baseUrl}{_options.SpaceId}", cancellationToken).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var space = jsonObject.ToObject<Space>();

            return space;
        }

        /// <summary>
        /// Gets a <see cref="ContentType"/> by the specified ID.
        /// </summary>
        /// <param name="contentTypeId">The ID of the content type.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into a <see cref="ContentType"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The <see name="contentTypeId">contentTypeId</see> parameter was null or empty</exception>
        public async Task<ContentType> GetContentTypeAsync(string contentTypeId, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(contentTypeId))
            {
                throw new ArgumentException(nameof(contentTypeId));
            }

            var res = await GetAsync($"{_baseUrl}{_options.SpaceId}/content_types/{contentTypeId}", cancellationToken).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var contentType = jsonObject.ToObject<ContentType>();

            return contentType;
        }

        /// <summary>
        /// Get all content types of a space.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ContentType"/>.</returns>
        public async Task<IEnumerable<ContentType>> GetContentTypesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var res = await GetAsync($"{_baseUrl}{_options.SpaceId}/content_types/", cancellationToken).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var contentTypes = jsonObject.SelectTokens("$..items[*]").Select(t => t.ToObject<ContentType>());

            return contentTypes;
        }

        /// <summary>
        /// Fetches an initial sync result of content. Note that this sync might not contain the entire result. 
        /// If the <see cref="SyncResult"/> returned contains a <see cref="SyncResult.NextPageUrl"/> that means 
        /// there are more resources to fetch. See also the <see cref="SyncInitialRecursiveAsync"/> method.
        /// </summary>
        /// <param name="syncType">The optional type of items that should be synced.</param>
        /// <param name="contentTypeId">The content type ID to filter entries by. Only applicable when the syncType is <see cref="SyncType.Entry"/>.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="SyncResult"/> containing all synced resources.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<SyncResult> SyncInitialAsync(SyncType syncType = SyncType.All, string contentTypeId = "", CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = BuildSyncQuery(syncType, contentTypeId, true);

            var res = await GetAsync($"{_baseUrl}{_options.SpaceId}/sync{query}", cancellationToken).ConfigureAwait(false);

            var syncResult = ParseSyncResultAsync(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return syncResult;
        }

        /// <summary>
        /// Syncs the delta changes since the last sync or the next page of an incomplete sync. 
        /// </summary>
        /// <param name="nextSyncOrPageUrl">The next page or next sync url from another <see cref="SyncResult"/>, 
        /// you can either pass the entire URL or only the syncToken query string parameter.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="SyncResult"/> containing all synced resources.</returns>
        /// <exception cref="ArgumentException">The <see name="nextSyncOrPageUrl">nextSyncOrPageUrl</see> parameter was null or empty</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<SyncResult> SyncNextResultAsync(string nextSyncOrPageUrl, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(nextSyncOrPageUrl))
            {
                throw new ArgumentException("nextPageUrl must be specified.", nameof(nextSyncOrPageUrl));
            }

            var syncToken = nextSyncOrPageUrl.Substring(nextSyncOrPageUrl.LastIndexOf('=') + 1);

            var query = BuildSyncQuery(syncToken:syncToken);

            var res = await GetAsync($"{_baseUrl}{_options.SpaceId}/sync{query}", cancellationToken).ConfigureAwait(false);

            var syncResult = ParseSyncResultAsync(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return syncResult;
        }

        /// <summary>
        /// Fetches an inital sync result of content and then recursively calls the api for any further 
        /// content available using the <see cref="SyncResult.NextPageUrl"/>. Note that this might result in
        /// multiple outgoing calls to the Contentful API. If you have a large amount of entries to sync consider using 
        /// the <see cref="SyncInitialAsync"/> method in conjunction with the <see cref="SyncNextResultAsync"/> method and 
        /// handling each response separately.
        /// </summary>
        /// <param name="syncType">The optional type of items that should be synced.</param>
        /// <param name="contentTypeId">The content type ID to filter entries by. Only applicable when the syncType is <see cref="SyncType.Entry"/>.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="SyncResult"/> containing all synced resources.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<SyncResult> SyncInitialRecursiveAsync(SyncType syncType = SyncType.All, string contentTypeId = "", CancellationToken cancellationToken = default(CancellationToken))
        {
            var syncResult = await SyncInitialAsync(syncType, contentTypeId).ConfigureAwait(false);

            while (!string.IsNullOrEmpty(syncResult.NextPageUrl))
            {
                var nextResult = await SyncNextResultAsync(syncResult.NextPageUrl, cancellationToken).ConfigureAwait(false);

                syncResult.Entries = syncResult.Entries.Concat(nextResult.Entries);
                syncResult.Assets = syncResult.Assets.Concat(nextResult.Assets);
                syncResult.DeletedAssets = syncResult.DeletedAssets.Concat(nextResult.DeletedAssets);
                syncResult.DeletedEntries = syncResult.DeletedEntries.Concat(nextResult.DeletedEntries);

                syncResult.SystemProperties = nextResult.SystemProperties;
                syncResult.NextPageUrl = nextResult.NextPageUrl;
                syncResult.NextSyncUrl = nextResult.NextSyncUrl;
            }

            return syncResult;
        }

        private SyncResult ParseSyncResultAsync(string content)
        {
            var jsonObject = JObject.Parse(content);
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

        private string BuildSyncQuery(SyncType syncType = SyncType.All, string contentTypeId = null, bool initial = false, string syncToken = null)
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

            if (!string.IsNullOrEmpty(syncToken))
            {
                querystringValues.Add(new KeyValuePair<string, string>("sync_token", syncToken));
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

        private async Task<HttpResponseMessage> GetAsync(string url, CancellationToken cancellationToken)
        {
            return await SendHttpRequestAsync(url, HttpMethod.Get, _options.DeliveryApiKey, cancellationToken).ConfigureAwait(false);
        }
    }
}
