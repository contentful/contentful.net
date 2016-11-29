using System.Collections.Generic;
using System.Threading.Tasks;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contentful.Core.Search;

namespace Contentful.Core
{
    public interface IContentfulManagementClient
    {
        Task<ContentType> ActivateContentTypeAsync(string contentTypeId, int version, string spaceId = null);
        Task<ManagementAsset> ArchiveAssetAsync(string assetId, int version, string spaceId = null);
        Task<Entry<dynamic>> ArchiveEntryAsync(string entryId, int version, string spaceId = null);
        Task<ApiKey> CreateApiKeyAsync(string name, string description, string spaceId = null);
        Task<Locale> CreateLocaleAsync(Locale locale, string spaceId = null);
        Task<ManagementAsset> CreateOrUpdateAssetAsync(ManagementAsset asset, string spaceId = null, int? version = default(int?));
        Task<ContentType> CreateOrUpdateContentTypeAsync(ContentType contentType, string spaceId = null, int? version = default(int?));
        Task<Entry<dynamic>> CreateOrUpdateEntryAsync(Entry<dynamic> entry, string spaceId = null, string contentTypeId = null, int? version = default(int?));
        Task<WebHook> CreateOrUpdateWebHookAsync(WebHook webhook, string spaceId = null);
        Task<Role> CreateRoleAsync(Role role, string spaceId = null);
        Task<Space> CreateSpaceAsync(string name, string defaultLocale, string organisation = null);
        Task<SpaceMembership> CreateSpaceMembershipAsync(SpaceMembership spaceMembership, string spaceId = null);
        Task<WebHook> CreateWebHookAsync(WebHook webhook, string spaceId = null);
        Task DeactivateContentTypeAsync(string contentTypeId, string spaceId = null);
        Task DeleteAssetAsync(string assetId, int version, string spaceId = null);
        Task DeleteContentTypeAsync(string contentTypeId, string spaceId = null);
        Task DeleteEntryAsync(string entryId, int version, string spaceId = null);
        Task DeleteLocaleAsync(string localeId, string spaceId = null);
        Task DeleteRoleAsync(string roleId, string spaceId = null);
        Task DeleteSpaceAsync(string id);
        Task DeleteSpaceMembershipAsync(string spaceMembershipId, string spaceId = null);
        Task DeleteWebHookAsync(string webhookId, string spaceId = null);
        Task<IEnumerable<ContentType>> GetActivatedContentTypesAsync(string spaceId = null);
        Task<ContentfulCollection<ApiKey>> GetAllApiKeysAsync(string spaceId = null);
        Task<ContentfulCollection<Role>> GetAllRolesAsync(string spaceId = null);
        Task<ContentfulCollection<Snapshot>> GetAllSnapshotsForEntryAsync(string entryId, string spaceId = null);
        Task<ManagementAsset> GetAssetAsync(string assetId, string spaceId = null);
        Task<ContentfulCollection<ManagementAsset>> GetAssetsCollectionAsync(string spaceId = null);
        Task<ContentType> GetContentTypeAsync(string contentTypeId, string spaceId = null);
        Task<IEnumerable<ContentType>> GetContentTypesAsync(string spaceId = null);
        Task<EditorInterface> GetEditorInterfaceAsync(string contentTypeId, string spaceId = null);
        Task<ContentfulCollection<T>> GetEntriesCollectionAsync<T>(QueryBuilder queryBuilder) where T : IContentfulResource;
        Task<ContentfulCollection<T>> GetEntriesCollectionAsync<T>(string queryString = null) where T : IContentfulResource;
        Task<Entry<dynamic>> GetEntryAsync(string entryId, string spaceId = null);
        Task<Locale> GetLocaleAsync(string localeId, string spaceId = null);
        Task<ContentfulCollection<Locale>> GetLocalesCollectionAsync(string spaceId = null);
        Task<ContentfulCollection<ManagementAsset>> GetPublishedAssetsCollectionAsync(string spaceId = null);
        Task<Role> GetRoleAsync(string roleId, string spaceId = null);
        Task<Snapshot> GetSnapshotForEntryAsync(string snapshotId, string entryId, string spaceId = null);
        Task<Space> GetSpaceAsync(string id);
        Task<SpaceMembership> GetSpaceMembershipAsync(string spaceMembershipId, string spaceId = null);
        Task<ContentfulCollection<SpaceMembership>> GetSpaceMembershipsAsync(string spaceId = null);
        Task<IEnumerable<Space>> GetSpacesAsync();
        Task<WebHook> GetWebHookAsync(string webhookId, string spaceId = null);
        Task<WebHookCallDetails> GetWebHookCallDetailsAsync(string callId, string webhookId, string spaceId = null);
        Task<ContentfulCollection<WebHookCallDetails>> GetWebHookCallDetailsCollectionAsync(string webhookId, string spaceId = null);
        Task<WebHookHealthResponse> GetWebHookHealthAsync(string webhookId, string spaceId = null);
        Task<ContentfulCollection<WebHook>> GetWebHooksCollectionAsync(string spaceId = null);
        Task ProcessAssetAsync(string assetId, int version, string locale, string spaceId = null);
        Task<ManagementAsset> PublishAssetAsync(string assetId, int version, string spaceId = null);
        Task<Entry<dynamic>> PublishEntryAsync(string entryId, int version, string spaceId = null);
        Task<ManagementAsset> UnarchiveAssetAsync(string assetId, int version, string spaceId = null);
        Task<Entry<dynamic>> UnarchiveEntryAsync(string entryId, int version, string spaceId = null);
        Task<ManagementAsset> UnpublishAssetAsync(string assetId, int version, string spaceId = null);
        Task<Entry<dynamic>> UnpublishEntryAsync(string entryId, int version, string spaceId = null);
        Task<EditorInterface> UpdateEditorInterfaceAsync(EditorInterface editorInterface, string contentTypeId, int version, string spaceId = null);
        Task<Locale> UpdateLocaleAsync(Locale locale, string spaceId = null);
        Task<Role> UpdateRoleAsync(Role role, string spaceId = null);
        Task<SpaceMembership> UpdateSpaceMembershipAsync(SpaceMembership spaceMembership, string spaceId = null);
        Task<Space> UpdateSpaceNameAsync(Space space, string organisation = null);
        Task<Space> UpdateSpaceNameAsync(string id, string name, int version, string organisation = null);
    }
}