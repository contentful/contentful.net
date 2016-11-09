using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contentful.Core.Models;
using Contentful.Core.Search;

namespace Contentful.Core
{
    /// <summary>
    /// Defines a set of methods to call underlying api endpoints at contentful.
    /// </summary>
    public interface IContentfulClient
    {
        Task<T> GetEntryAsync<T>(string entryId);
        Task<IEnumerable<T>> GetEntriesByType<T>(string contentTypeId, QueryBuilder queryBuilder = null);
        Task<IEnumerable<T>> GetEntriesAsync<T>(QueryBuilder queryBuilder);
        Task<IEnumerable<T>> GetEntriesAsync<T>(string queryString = null);
        Task<ContentfulCollection<T>> GetEntriesCollectionAsync<T>(QueryBuilder queryBuilder)
            where T : IContentfulResource;
        Task<ContentfulCollection<T>> GetEntriesCollectionAsync<T>(string queryString = null)
            where T : IContentfulResource;
        Task<Asset> GetAssetAsync(string assetId);
        Task<IEnumerable<Asset>> GetAssetsAsync(QueryBuilder queryBuilder);
        Task<IEnumerable<Asset>> GetAssetsAsync(string queryString = null);
        Task<ContentfulCollection<Asset>> GetAssetsCollectionAsync(QueryBuilder queryBuilder);
        Task<ContentfulCollection<Asset>> GetAssetsCollectionAsync(string queryString = null);
        Task<Space> GetSpaceAsync();
        Task<ContentType> GetContentTypeAsync(string contentTypeId);
        Task<IEnumerable<ContentType>> GetContentTypesAsync();
        Task<SyncResult> Sync(SyncType syncType = SyncType.All, string contentTypeId = "", bool initial = false);
        bool IsPreviewClient { get; }
    }
}
