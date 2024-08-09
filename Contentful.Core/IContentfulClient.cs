﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contentful.Core.Errors;
using Contentful.Core.Models;
using Contentful.Core.Search;
using System.Threading;
using Newtonsoft.Json;
using Contentful.Core.Configuration;
using Contentful.Core.Models.Management;

namespace Contentful.Core
{
    /// <summary>
    /// Defines a set of methods to call underlying api endpoints at contentful.
    /// </summary>
    public interface IContentfulClient
    {
        /// <summary>
        /// Gets or sets the resolver used when resolving entries to typed objects.
        /// </summary>
        IContentTypeResolver ContentTypeResolver { get; set; }

        /// <summary>
        /// Gets or sets the settings that should be used for deserialization.
        /// </summary>
        JsonSerializerSettings SerializerSettings { get; set; }
        
        /// <summary>
        /// Gets the serializer used by this client to deserialize content.
        /// </summary>
        public JsonSerializer Serializer { get; }

        /// <summary>
        /// Settings for the spaces to resolve cross space references from.
        /// </summary>
        List<CrossSpaceResolutionSetting> CrossSpaceResolutionSettings { get; set; }

        /// <summary>
        /// The base url used when calling the Contentful services. Defaults to https://cdn.contentful.com/spaces/
        /// </summary>
        string BaseUrl { get; set; }

        /// <summary>
        /// Get a single entry by the specified ID.
        /// </summary>
        /// <typeparam name="T">The type to serialize this entry into. If you want the metadata to 
        /// be included in the serialized response use the <see cref="Entry{T}"/> class as a type parameter.</typeparam>
        /// <param name="entryId">The ID of the entry.</param>
        /// <param name="etag">The e-tag to compare the server response to. If they match a null result will be returned.</param>
        /// <param name="queryBuilder">The optional <see cref="QueryBuilder{T}"/> to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A ContentfulResult with the deserialized Result and an Etag string, which is the e-tag returned from the server. Store this and pass it with
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The entryId parameter was null or empty.</exception>
        Task<ContentfulResult<T>> GetEntry<T>(string entryId, string etag, string queryString = null, CancellationToken cancellationToken = default);

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
        Task<T> GetEntry<T>(string entryId, QueryBuilder<T> queryBuilder, CancellationToken cancellationToken = default);

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
        Task<T> GetEntry<T>(string entryId, string queryString = null, CancellationToken cancellationToken = default);

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
        Task<ContentfulCollection<T>> GetEntriesByType<T>(string contentTypeId, QueryBuilder<T> queryBuilder = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all the entries of a space,filtered by an optional querystring.
        /// </summary>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of items.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        Task<string> GetEntriesRaw(string queryString = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all the entries of a space, filtered by an optional <see cref="QueryBuilder{T}"/>.
        /// </summary>
        /// <typeparam name="T">The class to serialize the response into. If you want the metadata to 
        /// be included in the serialized response use the <see cref="Entry{T}"/> class as a type parameter.</typeparam>
        /// <param name="queryBuilder">The optional <see cref="QueryBuilder{T}"/> to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of items.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        Task<ContentfulCollection<T>> GetEntries<T>(QueryBuilder<T> queryBuilder, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all the entries of a space, filtered by an optional querystring. A simpler approach than 
        /// to construct a query manually is to use the <see cref="QueryBuilder{T}"/> class.
        /// </summary>
        /// <typeparam name="T">The class to serialize the response into. If you want the metadata to 
        /// be included in the serialized response use the <see cref="Entry{T}"/> class as a type parameter.</typeparam>
        /// <param name="etag">The e-tag to compare the server response to. If they match a null result will be returned.</param>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of items.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        Task<ContentfulResult<ContentfulCollection<T>>> GetEntries<T>(string etag, string queryString = null, CancellationToken cancellationToken = default);

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
        Task<ContentfulCollection<T>> GetEntries<T>(string queryString = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a single <see cref="Asset"/> by the specified ID.
        /// </summary>
        /// <param name="assetId">The ID of the asset.</param>
        /// <param name="etag">The e-tag to compare the server response to. If they match a null result will be returned.</param>
        /// <param name="queryBuilder">The optional <see cref="QueryBuilder{T}"/> to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into an <see cref="Asset"/></returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The assetId parameter was null or emtpy.</exception>
        Task<ContentfulResult<Asset>> GetAsset(string assetId, string etag, string queryString = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a single <see cref="Asset"/> by the specified ID.
        /// </summary>
        /// <param name="assetId">The ID of the asset.</param>
        /// <param name="queryBuilder">The optional <see cref="QueryBuilder{T}"/> to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into an <see cref="Asset"/></returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The assetId parameter was null or emtpy.</exception>
        Task<Asset> GetAsset(string assetId, QueryBuilder<Asset> queryBuilder, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a single <see cref="Asset"/> by the specified ID.
        /// </summary>
        /// <param name="assetId">The ID of the asset.</param>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into an <see cref="Asset"/></returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The assetId parameter was null or emtpy.</exception>
        Task<Asset> GetAsset(string assetId, string queryString = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all assets of a space, filtered by an optional <see cref="QueryBuilder{T}"/>.
        /// </summary>
        /// <param name="etag">The e-tag to compare the server response to. If they match a null result will be returned.</param>
        /// <param name="queryBuilder">The optional <see cref="QueryBuilder{T}"/> to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Asset"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        Task<ContentfulResult<ContentfulCollection<Asset>>> GetAssets(string etag, string queryString = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all assets of a space, filtered by an optional <see cref="QueryBuilder{T}"/>.
        /// </summary>
        /// <param name="queryBuilder">The optional <see cref="QueryBuilder{T}"/> to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Asset"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        Task<ContentfulCollection<Asset>> GetAssets(QueryBuilder<Asset> queryBuilder, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all assets of a space, filtered by an optional querystring. A simpler approach than 
        /// to construct a query manually is to use the <see cref="QueryBuilder{T}"/> class.
        /// </summary>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Asset"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        Task<ContentfulCollection<Asset>> GetAssets(string queryString = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a key for signing embargoed asset requests.
        /// </summary>
        /// <param name="timeOffset">The point in time when the token should expire.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An instance of <see cref="EmbargoedAssetKey"/> with properties for signing embargoed asset urls.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <see name="timeOffset">timeOffset</see> parameter was in the past or more than 48 hours in the future.</exception>
        Task<EmbargoedAssetKey> CreateEmbargoedAssetKey(DateTimeOffset timeOffset, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the <see cref="Space"/> for this client.
        /// </summary>
        /// <param name="etag">The e-tag to compare the server response to. If they match a null result will be returned.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Space"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        Task<ContentfulResult<Space>> GetSpace(string etag, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the <see cref="Space"/> for this client.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Space"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        Task<Space> GetSpace(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a <see cref="ContentType"/> by the specified ID.
        /// </summary>
        /// <param name="etag">The e-tag to compare the server response to. If they match a null result will be returned.</param>
        /// <param name="contentTypeId">The ID of the content type.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into a <see cref="ContentType"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The contentTypeId parameter was null or empty</exception>
        Task<ContentfulResult<ContentType>> GetContentType(string etag, string contentTypeId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a <see cref="ContentType"/> by the specified ID.
        /// </summary>
        /// <param name="contentTypeId">The ID of the content type.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into a <see cref="ContentType"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="ArgumentException">The contentTypeId parameter was null or empty</exception>
        Task<ContentType> GetContentType(string contentTypeId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all content types of a space.
        /// </summary>
        /// <param name="etag">The e-tag to compare the server response to. If they match a null result will be returned.</param>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ContentType"/>.</returns>
        Task<ContentfulResult<IEnumerable<ContentType>>> GetContentTypes(string etag, string queryString, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all content types of a space.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ContentType"/>.</returns>
        Task<IEnumerable<ContentType>> GetContentTypes(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all content types of a space.
        /// </summary>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ContentType"/>.</returns>
        Task<IEnumerable<ContentType>> GetContentTypes(string queryString, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all locales of an environment.
        /// </summary>
        /// <param name="etag">The e-tag to compare the server response to. If they match a null result will be returned.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Locale"/>.</returns>
        Task<ContentfulResult<IEnumerable<Locale>>> GetLocales(string etag, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all locales of an environment.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Locale"/>.</returns>
        Task<IEnumerable<Locale>> GetLocales(CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all tags of an environment.
        /// </summary>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ContentTag"/>.</returns>
        Task<IEnumerable<ContentTag>> GetTags(string queryString = "", CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a tag of an environment by its id.
        /// </summary>
        /// <param name="tagId">The id of the tag to fetch.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentTag"/>.</returns>
        Task<ContentTag> GetTag(string tagId, CancellationToken cancellationToken = default);

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
        Task<SyncResult> SyncInitial(SyncType syncType = SyncType.All, string contentTypeId = "", CancellationToken cancellationToken = default, int? limit = null);

        /// <summary>
        /// Syncs the delta changes since the last sync or the next page of an incomplete sync. 
        /// </summary>
        /// <param name="nextSyncOrPageUrl">The next page or next sync url from another <see cref="SyncResult"/>, 
        /// you can either pass the entire URL or only the syncToken query string parameter.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="SyncResult"/> containing all synced resources.</returns>
        /// <exception cref="ArgumentException">The nextSyncOrPageUrl parameter was null or empty</exception>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        Task<SyncResult> SyncNextResult(string nextSyncOrPageUrl, CancellationToken cancellationToken = default);

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
        Task<SyncResult> SyncInitialRecursive(SyncType syncType = SyncType.All, string contentTypeId = "", CancellationToken cancellationToken = default, int? limit = null);

        /// <summary>
        /// Returns whether or not the client is using the preview API.
        /// </summary>
        bool IsPreviewClient { get; }
    }
}
