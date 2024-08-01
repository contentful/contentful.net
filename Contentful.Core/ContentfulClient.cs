using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Text;
using Contentful.Core.Search;
using System.Threading;
using Contentful.Core.Errors;
using Newtonsoft.Json;
using System.Collections;
using Contentful.Core.Models.Management;
using Contentful.Core.Extensions;

namespace Contentful.Core
{
    /// <summary>
    /// Main class for interaction with the contentful delivery and preview APIs
    /// </summary>
    public class ContentfulClient : ContentfulClientBase, IContentfulClient
    {
        private string _baseUrl = "https://cdn.contentful.com/spaces/";

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentfulClient"/> class. 
        /// The main class for interaction with the contentful deliver and preview APIs.
        /// </summary>
        /// <param name="httpClient">The HttpClient of your application.</param>
        /// <param name="options">The <see cref="ContentfulOptions"/> used for this client.</param>
        /// <exception cref="ArgumentException">The <see name="options">options</see> parameter was null or empty</exception>
        public ContentfulClient(HttpClient httpClient, ContentfulOptions options)
        {
            _httpClient = httpClient;
            _options = options;

            if (options == null)
            {
                throw new ArgumentException("The ContentfulOptions cannot be null.", nameof(options));
            }

            if (_options.UsePreviewApi)
            {
                BaseUrl = BaseUrl.Replace("cdn", "preview");
            }
            ResolveEntriesSelectively = _options.ResolveEntriesSelectively;
            SerializerSettings.Converters.Add(new AssetJsonConverter());
            SerializerSettings.Converters.Add(new ContentJsonConverter());
            SerializerSettings.TypeNameHandling = TypeNameHandling.All;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentfulClient"/> class.
        /// </summary>
        /// <param name="httpClient">The HttpClient of your application.</param>
        /// <param name="deliveryApiKey">The delivery API key used when communicating with the Contentful API.</param>
        /// <param name="previewApiKey">The preview API key used when communicating with the Contentful Preview API.</param>
        /// <param name="spaceId">The ID of the space to fetch content from.</param>
        /// <param name="usePreviewApi">Whether or not to use the Preview API for requests.
        /// If this is set to true the preview API key needs to be used for <paramref name="deliveryApiKey"/>
        ///  </param>
        public ContentfulClient(HttpClient httpClient, string deliveryApiKey, string previewApiKey, string spaceId, bool usePreviewApi = false) :
            this(httpClient, new ContentfulOptions()
            {
                DeliveryApiKey = deliveryApiKey,
                SpaceId = spaceId,
                PreviewApiKey = previewApiKey,
                UsePreviewApi = usePreviewApi
            })
        {

        }

        /// <summary>
        /// Returns whether or not the client is using the preview API.
        /// </summary>
        public bool IsPreviewClient => _options?.UsePreviewApi ?? false;

        /// <summary>
        /// Gets or sets the resolver used when resolving entries to typed objects.
        /// </summary>
        public IContentTypeResolver ContentTypeResolver { get; set; }

        /// <summary>
        /// If set the GetEntries methods will evaluate the class to serialize into and only serialize the parts that are part of the class structure.
        /// </summary>
        public bool ResolveEntriesSelectively { get; set; }

        /// <summary>
        /// Settings for the spaces to resolve cross space references from.
        /// </summary>
        public List<CrossSpaceResolutionSetting> CrossSpaceResolutionSettings { get; set; } = new List<CrossSpaceResolutionSetting>();

        /// <summary>
        /// The base url used when calling the Contentful services. Defaults to https://cdn.contentful.com/spaces/
        /// </summary>
        public string BaseUrl { get => _baseUrl; set => _baseUrl = value; }

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
        public async Task<T> GetEntry<T>(string entryId, QueryBuilder<T> queryBuilder, CancellationToken cancellationToken = default)
        {
            return await GetEntry<T>(entryId, queryBuilder?.Build(), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Get a single entry by the specified ID.
        /// </summary>
        /// <typeparam name="T">The type to serialize this entry into. If you want the metadata to 
        /// be included in the serialized response use the <see cref="Entry{T}"/> class as a type parameter.</typeparam>
        /// <param name="entryId">The ID of the entry.</param>
        /// <param name="etag">The e-tag to compare the server response to. If they match a null result will be returned.</param>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A ContentfulResult with the deserialized Result and an Etag string, which is the e-tag returned from the server. Store this and pass it with
        /// the next request to avoid unecessary calls if the content hasn't changed.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The entryId parameter was null or empty.</exception>
        public async Task<ContentfulResult<T>> GetEntry<T>(string entryId, string etag, string queryString = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(entryId))
            {
                throw new ArgumentException(nameof(entryId));
            }

            var res = await Get($"{BaseUrl}{_options.SpaceId}/{EnvironmentsBase}entries/{entryId}{queryString}", etag, cancellationToken, CrossSpaceResolutionSettings).ConfigureAwait(false);

            if(!string.IsNullOrEmpty(etag) && res.Headers?.ETag?.Tag == etag)
            {
                return new ContentfulResult<T>(res.Headers?.ETag?.Tag, default(T));
            }

            var json = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            JToken entry;

            //if T is Entry<U> then deserialize the object as is, so that the Fields property is the deserialization of the entry
            //otherwise move the sys object beneath the fields to make serialization more logical for the end user.
            var shouldCompact = !(typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Entry<>));

            if (shouldCompact)
            {
                entry = json.SelectToken("$.fields");
            }
            else
            {
                entry = json;
            }

            entry["sys"] = json.SelectToken("$.sys");
            entry["$metadata"] = json.SelectToken("$.metadata");

            var ob = entry.ToObject<T>(Serializer);

            return new ContentfulResult<T>(res.Headers?.ETag?.Tag ,ob);
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
        public async Task<T> GetEntry<T>(string entryId, string queryString = null, CancellationToken cancellationToken = default)
        {
            var ob = await GetEntry<T>(entryId, "", queryString, cancellationToken);

            return ob.Result;
        }

        /// <summary>
        /// Gets all the entries with the specified content type.
        /// </summary>
        /// <typeparam name="T">The class to serialize the response into. If you want the metadata to 
        /// be included in the serialized response use the <see cref="Entry{T}"/> class as a type parameter.</typeparam>
        /// <param name="contentTypeId">The ID of the content type to get entries for.</param>
        /// <param name="queryBuilder">The optional <see cref="QueryBuilder{T}"/> to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of items.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<T>> GetEntriesByType<T>(string contentTypeId, QueryBuilder<T> queryBuilder = null, CancellationToken cancellationToken = default)
        {
            var builder = queryBuilder ?? new QueryBuilder<T>();
            builder.ContentTypeIs(contentTypeId);

            return await GetEntries<T>(builder, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all the entries of a space, filtered by an optional <see cref="QueryBuilder{T}"/>.
        /// </summary>
        /// <typeparam name="T">The class to serialize the response into. If you want the metadata to 
        /// be included in the serialized response use the <see cref="Entry{T}"/> class as a type parameter.</typeparam>
        /// <param name="queryBuilder">The optional <see cref="QueryBuilder{T}"/> to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of items.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<T>> GetEntries<T>(QueryBuilder<T> queryBuilder, CancellationToken cancellationToken = default)
        {
            return await GetEntries<T>(queryBuilder?.Build(), cancellationToken).ConfigureAwait(false);
        }

        public async Task<string> GetEntriesRaw(string queryString = null, CancellationToken cancellationToken = default)
        {
            var res = await Get($"{BaseUrl}{_options.SpaceId}/{EnvironmentsBase}entries{queryString}", null, cancellationToken, CrossSpaceResolutionSettings).ConfigureAwait(false);
            var resultString = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
            return resultString;
        }

        public async Task<ContentfulResult<ContentfulCollection<T>>> GetEntries<T>(string etag, string queryString = null, CancellationToken cancellationToken = default)
        {
            var res = await Get($"{BaseUrl}{_options.SpaceId}/{EnvironmentsBase}entries{queryString}", etag, cancellationToken, CrossSpaceResolutionSettings).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(etag) && res.Headers?.ETag.Tag == etag)
            {
                return new ContentfulResult<ContentfulCollection<T>>(res.Headers?.ETag?.Tag, null);
            }

            var json = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            ReplaceMetaData(json);

            var processedIds = new HashSet<string>();
            var errors = json.SelectToken("$.errors");
            if (errors == null)
            {
                // the errors array was not present, create it.
                json.Add("errors", new JArray());
            }
            foreach (var item in json.SelectTokens("$.items[*]").OfType<JObject>())
            {
                ResolveLinks(json, item, processedIds, new HashSet<string>(), typeof(T));
            }

            var entryTokens = json.SelectTokens("$.items[*]..fields").ToList();

            for (var i = entryTokens.Count - 1; i >= 0; i--)
            {
                var token = entryTokens[i];
                var grandParent = token.Parent.Parent;

                if (grandParent["sys"]?["type"] != null && grandParent["sys"]["type"]?.ToString() != "Entry" || token.Parent.Path.EndsWith(".fields.fields"))
                {
                    continue;
                }

                ResolveContentTypes(grandParent);

                //Remove the fields property and let the fields be direct descendants of the node to make deserialization logical.
                token.Parent.Remove();
                grandParent.Add(token.Children());
            }

            var collection = json.ToObject<ContentfulCollection<T>>(Serializer);

            collection.IncludedAssets = json.SelectTokens("$.includes.Asset[*]")?.Select(t => t.ToObject<Asset>(Serializer));

            collection.IncludedEntries = json.SelectTokens("$.includes.Entry[*]")?.Select(t => t.ToObject<Entry<dynamic>>(Serializer));

            return new ContentfulResult<ContentfulCollection<T>>(res.Headers?.ETag?.Tag, collection);
        }

        /// <summary>
        /// Gets all the entries of a space, filtered by an optional querystring. A simpler approach than 
        /// to construct a query manually is to use the <see cref="QueryBuilder{T}"/> class.
        /// </summary>
        /// <typeparam name="T">The class to serialize the response into. If you want the metadata to 
        /// be included in the serialized response use the <see cref="Entry{T}"/> class as a type parameter.</typeparam>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of items.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<T>> GetEntries<T>(string queryString = null, CancellationToken cancellationToken = default)
        {
            var res = await GetEntries<T>("", queryString, cancellationToken);

            return res.Result;
        }

        private void ResolveContentTypes(JContainer container)
        {
            if (ContentTypeResolver == null || container["$type"] != null)
            {
                return;
            }

            var contentType = container["sys"]?["contentType"]?["sys"]?["id"]?.ToString();

            if (contentType == null)
            {
                return;
            }

            var type = ContentTypeResolver.Resolve(contentType);

            if (type != null)
            {
                container.AddFirst(new JProperty("$type", type.AssemblyQualifiedName));
            }
        }

        private void ResolveLinks(JObject json, JObject entryToken, ISet<string> processedIds, ISet<string> scopedIds, Type type)
        {
            var id = ((JValue)entryToken.SelectToken("$.sys.id"))?.Value?.ToString();

            if (id == null)
            {
                id = ((JValue)entryToken.SelectToken("$.data.target.sys.id"))?.Value?.ToString();
            }

            if (id == null)
            {
                //No id token present, not possible to resolve links. Probably because the sys property has been excluded with a select statement.
                return;
            }

            ResolveContentTypes(entryToken);

            if (entryToken["$type"] != null)
            {
                type = Type.GetType(entryToken["$type"].Value<string>());
            }

            if (!processedIds.Contains(id))
            {
                entryToken.AddFirst(new JProperty("$id", new JValue(id)));
                processedIds.Add(id);
            }

            scopedIds.Add(id);

            var links = entryToken.SelectTokens("$.fields..sys").ToList();
            //Walk through and add any included entries as direct links.
            foreach (var linkToken in links)
            {
                var propName = linkToken.Path.Substring(linkToken.Path.LastIndexOf(".fields.") + 8);
                propName = propName.Substring(0, propName.IndexOf("."));
                //remove any [] from propname if it's a collection property
                if (propName.IndexOf("[") > 0)
                {
                    propName = propName.Substring(0, propName.IndexOf("["));
                }
                var linkId = "";
                var linktype = linkToken["linkType"]?.ToString();
                if (linkToken["type"]?.ToString() == "ResourceLink")
                {
                    if (!CrossSpaceResolutionSettings.Any())
                    {
                        //There are no cross spaces configured, the client will have to handle this node by themselves.
                        continue;
                    }
                    linkId = ((JValue)linkToken["urn"]).Value.ToString();
                    linkId = ParseIdFromContentfulUrn(linkId);
                    linktype = linktype.Contains("Entry") ? "Entry" : "Asset";
                } else {
                    linkId = ((JValue)linkToken["id"]).Value.ToString();
                }

                JToken replacementToken = null;
                if (scopedIds.Contains(linkId))
                {
                    replacementToken = new JObject
                    {
                        ["$ref"] = linkId
                    };
                }
                else if (!string.IsNullOrEmpty(linktype))
                {
                    replacementToken = json.SelectTokens($"$.includes.{linktype}[?(@.sys.id=='{linkId}')]").FirstOrDefault();

                    if (replacementToken == null)
                    {
                        //This could be due to the referenced entry being part of the original request (circular reference), so scan through that as well.
                        replacementToken = json.SelectTokens($"$.items.[?(@.sys.id=='{linkId}')]").FirstOrDefault();
                    }


                }

                var grandParent = (JObject)linkToken.Parent.Parent;

                if (replacementToken != null)
                {
                    grandParent.RemoveAll();
                    grandParent.Add(replacementToken.Children());
                    PropertyInfo prop = null;

                    if (ResolveEntriesSelectively)
                    {
                        prop = type?.GetRuntimeProperties().FirstOrDefault(p => (p.Name.Equals(propName, StringComparison.OrdinalIgnoreCase) ||
                        p.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName == propName));
                        if (prop == null && linktype?.ToString() != "Asset")
                        {
                            //the property does not exist in the entry. Skip it in resolving references.
                            continue;
                        }
                    }

                    if (!processedIds.Contains(linkId))
                    {
                        Type propType = null;

                        if (ResolveEntriesSelectively)
                        {
                            propType = prop?.PropertyType;

                            if (propType != null && propType.IsArray)
                            {
                                propType = propType.GetElementType();
                            }
                            else if (propType != null && typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(propType.GetTypeInfo()) && propType.IsConstructedGenericType)
                            {
                                propType = propType.GetTypeInfo().GenericTypeArguments[0];
                            }
                        }

                        ResolveLinks(json, grandParent, processedIds, new HashSet<string>(scopedIds), propType);
                    }
                }
                else
                {
                    var errorToken = json.SelectTokens($"$.errors.[?(@.details.id=='{linkId}')]").FirstOrDefault();

                    // The include is missing and present in the errors (possibly it was removed in contentful), we skip it to make sure it deserializes to null
                    if (errorToken != null)
                    {
                        var itemToSkip = grandParent.Parent is JProperty ? grandParent.Parent : grandParent;
                        itemToSkip.Remove();
                    }
                    else
                    {
                        // The include is missing and not present in errors. Possibly because the includes parameter is set to too low a value.
                        // We skip it to make sure it can still deserialize the entire structure, but also add it to the errors.

                        var itemToSkip = grandParent.Parent is JProperty ? grandParent.Parent : grandParent;

                        var errors = json.SelectToken("$.errors");
                        var newError = new ContentfulError
                        {
                            SystemProperties = new SystemProperties
                            {
                                Id = "notResolvable",
                                Type = "error"
                            },
                            Details = new ContentfulErrorDetails
                            {
                                Type = "Link",
                                LinkType = itemToSkip.SelectToken("$..sys.linkType")?.Value<string>(),
                                Id = linkId
                            }
                        };


                        (errors as JArray).Add(JObject.FromObject(newError));

                        itemToSkip.Remove();
                    }
                }
            }
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
        public async Task<Asset> GetAsset(string assetId, QueryBuilder<Asset> queryBuilder, CancellationToken cancellationToken = default)
        {
            return await GetAsset(assetId, queryBuilder?.Build(), cancellationToken);
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
        public async Task<ContentfulResult<Asset>> GetAsset(string assetId, string etag, string queryString = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(assetId))
            {
                throw new ArgumentException(nameof(assetId));
            }

            var res = await Get($"{BaseUrl}{_options.SpaceId}/{EnvironmentsBase}assets/{assetId}{queryString}", etag, cancellationToken, null).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(etag) && res.Headers?.ETag?.Tag == etag)
            {
                return new ContentfulResult<Asset>(res.Headers?.ETag?.Tag, null);
            }

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var asset = jsonObject.ToObject<Asset>(Serializer);

            return new ContentfulResult<Asset>(res.Headers?.ETag?.Tag, asset);
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
        public async Task<Asset> GetAsset(string assetId, string queryString = null, CancellationToken cancellationToken = default)
        {
            var asset = await GetAsset(assetId, "", queryString, cancellationToken).ConfigureAwait(false);

            return asset.Result;
        }

        /// <summary>
        /// Gets all assets of a space, filtered by an optional <see cref="QueryBuilder{T}"/>.
        /// </summary>
        /// <param name="queryBuilder">The optional <see cref="QueryBuilder{T}"/> to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Asset"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<Asset>> GetAssets(QueryBuilder<Asset> queryBuilder, CancellationToken cancellationToken = default)
        {
            return await GetAssets(queryBuilder?.Build(), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all assets of a space, filtered by an optional queryString. A simpler approach than 
        /// to construct a query manually is to use the <see cref="QueryBuilder{T}"/> class.
        /// </summary>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Asset"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulResult<ContentfulCollection<Asset>>> GetAssets(string etag, string queryString = null, CancellationToken cancellationToken = default)
        {
            var res = await Get($"{BaseUrl}{_options.SpaceId}/{EnvironmentsBase}assets/{queryString}", etag, cancellationToken, null).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(etag) && res.Headers?.ETag?.Tag == etag)
            {
                return new ContentfulResult<ContentfulCollection<Asset>>(res.Headers?.ETag?.Tag, null);
            }

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var collection = jsonObject.ToObject<ContentfulCollection<Asset>>(Serializer);
            var assets = jsonObject.SelectTokens("$.items[*]").Select(c => c.ToObject<Asset>(Serializer)); ;
            collection.Items = assets;
            return new ContentfulResult<ContentfulCollection<Asset>>(res.Headers?.ETag?.Tag, collection);
        }

        /// <summary>
        /// Gets all assets of a space, filtered by an optional queryString. A simpler approach than 
        /// to construct a query manually is to use the <see cref="QueryBuilder{T}"/> class.
        /// </summary>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Asset"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulCollection<Asset>> GetAssets(string queryString = null, CancellationToken cancellationToken = default)
        {
            var collection = await GetAssets("", queryString, cancellationToken);
            return collection.Result;
        }

        /// <summary>
        /// Creates a key for signing embargoed asset requests.
        /// </summary>
        /// <param name="timeOffset">The point in time when the token should expire.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An instance of <see cref="EmbargoedAssetKey"/> with properties for signing embargoed asset urls.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <see name="timeOffset">timeOffset</see> parameter was in the past or more than 48 hours in the future.</exception>
        public async Task<EmbargoedAssetKey> CreateEmbargoedAssetKey(DateTimeOffset timeOffset, CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;

            if (timeOffset < now)
            {
                throw new ArgumentOutOfRangeException("The asset key expiration must be in the future.", nameof(timeOffset));
            }

           //Should test that timeOffset is less than 48 hours in the future.
           //but for now we will let the api handle it since we don't have a clear mock of current time.

            var expiresAt = timeOffset.ToUnixTimeSeconds();
            //note that unlike some other api methods, the asset embargo key always requires an environment id.
            var res = await Post($"{BaseUrl}{_options.SpaceId}/{(EnvironmentsBase == string.Empty ? "environments/master/" : EnvironmentsBase)}asset_keys", new { expiresAt }, cancellationToken).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var assetKey = jsonObject.ToObject<EmbargoedAssetKey>(Serializer);
            assetKey.ExpiresAtUtc = timeOffset.UtcDateTime;
            return assetKey;
        }

        /// <summary>
        /// Gets the <see cref="Space"/> for this client.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Space"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<ContentfulResult<Space>> GetSpace(string etag, CancellationToken cancellationToken = default)
        {
            var res = await Get($"{BaseUrl}{_options.SpaceId}", etag, cancellationToken, null).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(etag) && res.Headers?.ETag?.Tag == etag)
            {
                return new ContentfulResult<Space>(res.Headers?.ETag?.Tag, null);
            }

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var space = jsonObject.ToObject<Space>(Serializer);

            return new ContentfulResult<Space>(res.Headers?.ETag?.Tag, space);
        }


        /// <summary>
        /// Gets the <see cref="Space"/> for this client.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Space"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<Space> GetSpace(CancellationToken cancellationToken = default)
        {
            var res = await GetSpace("", cancellationToken);

            return res.Result;
        }

        /// <summary>
        /// Gets a <see cref="ContentType"/> by the specified ID.
        /// </summary>
        /// <param name="contentTypeId">The ID of the content type.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into a <see cref="ContentType"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The <see name="contentTypeId">contentTypeId</see> parameter was null or empty</exception>
        public async Task<ContentfulResult<ContentType>> GetContentType(string etag, string contentTypeId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(contentTypeId))
            {
                throw new ArgumentException(nameof(contentTypeId));
            }

            var res = await Get($"{BaseUrl}{_options.SpaceId}/{EnvironmentsBase}content_types/{contentTypeId}", etag, cancellationToken, null).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(etag) && res.Headers?.ETag?.Tag == etag)
            {
                return new ContentfulResult<ContentType>(res.Headers?.ETag?.Tag, null);
            }

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var contentType = jsonObject.ToObject<ContentType>(Serializer);

            return new ContentfulResult<ContentType>(res.Headers?.ETag?.Tag, contentType);
        }

        /// <summary>
        /// Get all tags of an environment.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ContentTag"/>.</returns>
        public async Task<IEnumerable<ContentTag>> GetTags(string queryString = "", CancellationToken cancellationToken = default) {

            var res = await Get($"{BaseUrl}{_options.SpaceId}/{EnvironmentsBase}tags/{queryString}", null, cancellationToken, null).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var tags = jsonObject.SelectTokens("$..items[*]").Select(t => t.ToObject<ContentTag>(Serializer));

            return tags;

        }

        /// <summary>
        /// Get a tag of an environment by its id.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Locale"/>.</returns>
        public async Task<ContentTag> GetTag(string tagId, CancellationToken cancellationToken = default) {

            if (string.IsNullOrEmpty(tagId))
            {
                throw new ArgumentException(nameof(tagId));
            }

            var res = await Get($"{BaseUrl}{_options.SpaceId}/{EnvironmentsBase}tags/{tagId}", null, cancellationToken, null).ConfigureAwait(false);

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var tag = jsonObject.ToObject<ContentTag>(Serializer);

            return tag;
        }

        /// <summary>
        /// Gets a <see cref="ContentType"/> by the specified ID.
        /// </summary>
        /// <param name="contentTypeId">The ID of the content type.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into a <see cref="ContentType"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The <see name="contentTypeId">contentTypeId</see> parameter was null or empty</exception>
        public async Task<ContentType> GetContentType(string contentTypeId, CancellationToken cancellationToken = default)
        {
            var contentType = await GetContentType("", contentTypeId, cancellationToken);

            return contentType.Result;
        }

        /// <summary>
        /// Get all content types of a space.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ContentType"/>.</returns>
        public async Task<IEnumerable<ContentType>> GetContentTypes(CancellationToken cancellationToken = default)
        {
            return await GetContentTypes(null, cancellationToken);
        }

        /// <summary>
        /// Get all content types of a space.
        /// </summary>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ContentType"/>.</returns>
        public async Task<ContentfulResult<IEnumerable<ContentType>>> GetContentTypes(string etag, string queryString, CancellationToken cancellationToken = default)
        {
            var res = await Get($"{BaseUrl}{_options.SpaceId}/{EnvironmentsBase}content_types/{queryString}", etag, cancellationToken, null).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(etag) && res.Headers?.ETag?.Tag == etag)
            {
                return new ContentfulResult<IEnumerable<ContentType>>(res.Headers?.ETag?.Tag, null);
            }

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var contentTypes = jsonObject.SelectTokens("$..items[*]").Select(t => t.ToObject<ContentType>(Serializer));

            return new ContentfulResult<IEnumerable<ContentType>>(res.Headers?.ETag?.Tag, contentTypes);
        }

        /// <summary>
        /// Get all content types of a space.
        /// </summary>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ContentType"/>.</returns>
        public async Task<IEnumerable<ContentType>> GetContentTypes(string queryString, CancellationToken cancellationToken = default)
        {
            var contentTypes = await GetContentTypes("", queryString, cancellationToken);

            return contentTypes.Result;
        }

        /// <summary>
        /// Get all locales of an environment.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Locale"/>.</returns>
        public async Task<ContentfulResult<IEnumerable<Locale>>> GetLocales(string etag, CancellationToken cancellationToken = default)
        {
            var res = await Get($"{BaseUrl}{_options.SpaceId}/{(string.IsNullOrEmpty(EnvironmentsBase) ? "environments/master/" : EnvironmentsBase)}locales/", etag, cancellationToken, null).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(etag) && res.Headers?.ETag?.Tag == etag)
            {
                return new ContentfulResult<IEnumerable<Locale>> (res.Headers?.ETag?.Tag, null);
            }

            var jsonObject = JObject.Parse(await res.Content.ReadAsStringAsync().ConfigureAwait(false));
            var locales = jsonObject.SelectTokens("$..items[*]").Select(t => t.ToObject<Locale>(Serializer));

            return new ContentfulResult<IEnumerable<Locale>>(res.Headers?.ETag?.Tag, locales);
        }

        /// <summary>
        /// Get all locales of an environment.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Locale"/>.</returns>
        public async Task<IEnumerable<Locale>> GetLocales(CancellationToken cancellationToken = default)
        {
            var locales = await GetLocales("", cancellationToken);

            return locales.Result;
        }

        /// <summary>
        /// Fetches an initial sync result of content. Note that this sync might not contain the entire result. 
        /// If the <see cref="SyncResult"/> returned contains a <see cref="SyncResult.NextPageUrl"/> that means 
        /// there are more resources to fetch. See also the <see cref="SyncInitialRecursive"/> method.
        /// </summary>
        /// <param name="syncType">The optional type of items that should be synced.</param>
        /// <param name="contentTypeId">The content type ID to filter entries by. Only applicable when the syncType is <see cref="SyncType.Entry"/>.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <param name="limit">Limits the number of items returned on each page of the sync result.</param>
        /// <returns>A <see cref="SyncResult"/> containing all synced resources.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<SyncResult> SyncInitial(SyncType syncType = SyncType.All, string contentTypeId = "", CancellationToken cancellationToken = default, int? limit = null)
        {
            if (!string.IsNullOrEmpty(contentTypeId) && syncType != SyncType.Entry)
            {
                throw new ArgumentException("A content type can only be specified at the initial sync and only if the sync type is Entry. Here the synctype was " + syncType);
            }

            var query = BuildSyncQuery(syncType, contentTypeId, true, limit: limit);

            var res = await Get($"{BaseUrl}{_options.SpaceId}/{EnvironmentsBase}sync{query}", "", cancellationToken, null).ConfigureAwait(false);

            var syncResult = ParseSyncResult(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

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
        public async Task<SyncResult> SyncNextResult(string nextSyncOrPageUrl, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(nextSyncOrPageUrl))
            {
                throw new ArgumentException("nextPageUrl must be specified.", nameof(nextSyncOrPageUrl));
            }

            var syncToken = nextSyncOrPageUrl.Substring(nextSyncOrPageUrl.LastIndexOf('=') + 1);

            var query = BuildSyncQuery(syncToken: syncToken);

            var res = await Get($"{BaseUrl}{_options.SpaceId}/{EnvironmentsBase}sync{query}", "", cancellationToken, null).ConfigureAwait(false);

            var syncResult = ParseSyncResult(await res.Content.ReadAsStringAsync().ConfigureAwait(false));

            return syncResult;
        }

        /// <summary>
        /// Fetches an inital sync result of content and then recursively calls the api for any further 
        /// content available using the <see cref="SyncResult.NextPageUrl"/>. Note that this might result in
        /// multiple outgoing calls to the Contentful API. If you have a large amount of entries to sync consider using 
        /// the <see cref="SyncInitial"/> method in conjunction with the <see cref="SyncNextResult"/> method and 
        /// handling each response separately.
        /// </summary>
        /// <param name="syncType">The optional type of items that should be synced.</param>
        /// <param name="contentTypeId">The content type ID to filter entries by. Only applicable when the syncType is <see cref="SyncType.Entry"/>.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <param name="limit">Limits the number of items returned on each page of the sync result.</param>
        /// <returns>A <see cref="SyncResult"/> containing all synced resources.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        public async Task<SyncResult> SyncInitialRecursive(SyncType syncType = SyncType.All, string contentTypeId = "", CancellationToken cancellationToken = default, int? limit = null)
        {
            var syncResult = await SyncInitial(syncType, contentTypeId, cancellationToken, limit).ConfigureAwait(false);

            while (!string.IsNullOrEmpty(syncResult.NextPageUrl))
            {
                var nextResult = await SyncNextResult(syncResult.NextPageUrl, cancellationToken).ConfigureAwait(false);

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

        private SyncResult ParseSyncResult(string content)
        {
            var jsonObject = JObject.Parse(content);

            ReplaceMetaData(jsonObject);

            var syncResult = jsonObject.ToObject<SyncResult>(Serializer);
            var entries =
                jsonObject.SelectTokens("$.items[?(@.sys.type=='Entry')]").Select(c => c.ToObject<Entry<dynamic>>(Serializer));
            var assets =
                jsonObject.SelectTokens("$.items[?(@.sys.type=='Asset')]").Select(c => c.ToObject<SyncedAsset>(Serializer));
            var deletedEntries = jsonObject.SelectTokens("$.items[?(@.sys.type=='DeletedEntry')].sys").Select(c => c.ToObject<SystemProperties>(Serializer));
            var deletedAssets = jsonObject.SelectTokens("$.items[?(@.sys.type=='DeletedAsset')].sys").Select(c => c.ToObject<SystemProperties>(Serializer));

            syncResult.Assets = assets;
            syncResult.DeletedAssets = deletedAssets;
            syncResult.DeletedEntries = deletedEntries;
            syncResult.Entries = entries;

            return syncResult;
        }

        private string BuildSyncQuery(SyncType syncType = SyncType.All, string contentTypeId = null, bool initial = false, string syncToken = null, int? limit = null)
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

            if (limit.HasValue)
            {
                querystringValues.Add(new KeyValuePair<string, string>("limit", limit.Value.ToString()));

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

        private async Task<HttpResponseMessage> Get(string url, string etag, CancellationToken cancellationToken, List<CrossSpaceResolutionSetting> crossSpaceReferences)
        {
            var headers = new List<KeyValuePair<string, IEnumerable<string>>>();
            if (!string.IsNullOrEmpty(etag))
            {
                headers.Add(new KeyValuePair<string, IEnumerable<string>>("If-None-Match", new List<string> { etag }));
            }
            if(crossSpaceReferences != null && crossSpaceReferences.Count > 0)
            {
                dynamic exo = new System.Dynamic.ExpandoObject();

                foreach (var item in crossSpaceReferences)
                {
                    ((IDictionary<string, object>)exo).Add(item.SpaceId, item.CdaToken);
                }

                var headerObject = new { spaces = exo };
                var jsonString = JsonConvert.SerializeObject(headerObject);
                var base64EncodedValue = System.Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonString));

                headers.Add(new KeyValuePair<string, IEnumerable<string>>("x-contentful-resource-resolution", new List<string> { base64EncodedValue }));
            }
            return await SendHttpRequest(url, HttpMethod.Get, _options.UsePreviewApi ? _options.PreviewApiKey : _options.DeliveryApiKey, cancellationToken, additionalHeaders: headers).ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> Post(string url, object body, CancellationToken cancellationToken)
        {
            var bodyJson = body.ConvertObjectToJsonString();
            var bodyContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");
            return await SendHttpRequest(url, HttpMethod.Post, _options.UsePreviewApi ? _options.PreviewApiKey : _options.DeliveryApiKey, cancellationToken, bodyContent).ConfigureAwait(false);
        }

        private string ParseIdFromContentfulUrn(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;
            return s.Substring(s.LastIndexOf('/') + 1);
        }
    }
}
