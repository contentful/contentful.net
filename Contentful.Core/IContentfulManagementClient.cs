using System.Collections.Generic;
using System.Threading.Tasks;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contentful.Core.Search;
using System.Threading;

namespace Contentful.Core
{
    public interface IContentfulManagementClient
    {
        Task<ContentType> ActivateContentTypeAsync(string contentTypeId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<ManagementAsset> ArchiveAssetAsync(string assetId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<Entry<dynamic>> ArchiveEntryAsync(string entryId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<ApiKey> CreateApiKeyAsync(string name, string description, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<Locale> CreateLocaleAsync(Locale locale, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<ManagementAsset> CreateOrUpdateAssetAsync(ManagementAsset asset, string spaceId = null, int? version = default(int?), CancellationToken cancellationToken = default(CancellationToken));
        Task<ContentType> CreateOrUpdateContentTypeAsync(ContentType contentType, string spaceId = null, int? version = default(int?), CancellationToken cancellationToken = default(CancellationToken));
        Task<Entry<dynamic>> CreateOrUpdateEntryAsync(Entry<dynamic> entry, string spaceId = null, string contentTypeId = null, int? version = default(int?), CancellationToken cancellationToken = default(CancellationToken));
        Task<WebHook> CreateOrUpdateWebHookAsync(WebHook webhook, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<Role> CreateRoleAsync(Role role, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<Space> CreateSpaceAsync(string name, string defaultLocale, string organisation = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<SpaceMembership> CreateSpaceMembershipAsync(SpaceMembership spaceMembership, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<WebHook> CreateWebHookAsync(WebHook webhook, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task DeactivateContentTypeAsync(string contentTypeId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task DeleteAssetAsync(string assetId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task DeleteContentTypeAsync(string contentTypeId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task DeleteEntryAsync(string entryId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task DeleteLocaleAsync(string localeId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task DeleteRoleAsync(string roleId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task DeleteSpaceAsync(string id, CancellationToken cancellationToken = default(CancellationToken));
        Task DeleteSpaceMembershipAsync(string spaceMembershipId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task DeleteWebHookAsync(string webhookId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<IEnumerable<ContentType>> GetActivatedContentTypesAsync(string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<ContentfulCollection<ApiKey>> GetAllApiKeysAsync(string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<ContentfulCollection<Role>> GetAllRolesAsync(string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<ContentfulCollection<Snapshot>> GetAllSnapshotsForEntryAsync(string entryId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<ManagementAsset> GetAssetAsync(string assetId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<ContentfulCollection<ManagementAsset>> GetAssetsCollectionAsync(string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<ContentType> GetContentTypeAsync(string contentTypeId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<IEnumerable<ContentType>> GetContentTypesAsync(string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<EditorInterface> GetEditorInterfaceAsync(string contentTypeId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<ContentfulCollection<T>> GetEntriesCollectionAsync<T>(QueryBuilder<T> queryBuilder, CancellationToken cancellationToken = default(CancellationToken)) where T : IContentfulResource;
        Task<ContentfulCollection<T>> GetEntriesCollectionAsync<T>(string queryString = null, CancellationToken cancellationToken = default(CancellationToken)) where T : IContentfulResource;
        Task<Entry<dynamic>> GetEntryAsync(string entryId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<Locale> GetLocaleAsync(string localeId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<ContentfulCollection<Locale>> GetLocalesCollectionAsync(string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<ContentfulCollection<ManagementAsset>> GetPublishedAssetsCollectionAsync(string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<Role> GetRoleAsync(string roleId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<Snapshot> GetSnapshotForEntryAsync(string snapshotId, string entryId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<Space> GetSpaceAsync(string id, CancellationToken cancellationToken = default(CancellationToken));
        Task<SpaceMembership> GetSpaceMembershipAsync(string spaceMembershipId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<ContentfulCollection<SpaceMembership>> GetSpaceMembershipsAsync(string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<IEnumerable<Space>> GetSpacesAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task<WebHook> GetWebHookAsync(string webhookId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<WebHookCallDetails> GetWebHookCallDetailsAsync(string callId, string webhookId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<ContentfulCollection<WebHookCallDetails>> GetWebHookCallDetailsCollectionAsync(string webhookId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<WebHookHealthResponse> GetWebHookHealthAsync(string webhookId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<ContentfulCollection<WebHook>> GetWebHooksCollectionAsync(string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task ProcessAssetAsync(string assetId, int version, string locale, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<ManagementAsset> PublishAssetAsync(string assetId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<Entry<dynamic>> PublishEntryAsync(string entryId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<ManagementAsset> UnarchiveAssetAsync(string assetId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<Entry<dynamic>> UnarchiveEntryAsync(string entryId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<ManagementAsset> UnpublishAssetAsync(string assetId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<Entry<dynamic>> UnpublishEntryAsync(string entryId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<EditorInterface> UpdateEditorInterfaceAsync(EditorInterface editorInterface, string contentTypeId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<Locale> UpdateLocaleAsync(Locale locale, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<Role> UpdateRoleAsync(Role role, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<SpaceMembership> UpdateSpaceMembershipAsync(SpaceMembership spaceMembership, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<Space> UpdateSpaceNameAsync(Space space, string organisation = null, CancellationToken cancellationToken = default(CancellationToken));
        Task<Space> UpdateSpaceNameAsync(string id, string name, int version, string organisation = null, CancellationToken cancellationToken = default(CancellationToken));
    }
}