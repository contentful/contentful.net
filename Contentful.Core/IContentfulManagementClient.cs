using System.Collections.Generic;
using System.Threading.Tasks;
using Contentful.Core.Models;
using Contentful.Core.Models.Management;
using Contentful.Core.Search;
using System.Threading;

namespace Contentful.Core
{
    /// <summary>
    /// Interface for methods to interact with the Contentful Management API.
    /// </summary>
    public interface IContentfulManagementClient
    {
        /// <summary>
        /// Activates a <see cref="ContentType"/> by the specified id.
        /// </summary>
        /// <param name="contentTypeId">The id of the content type.</param>
        /// <param name="version">The last known version of the content type.</param>
        /// <param name="spaceId">The id of the space to activate the content type in. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into a <see cref="ContentType"/>.</returns>
        Task<ContentType> ActivateContentTypeAsync(string contentTypeId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Archives an asset by the specified id.
        /// </summary>
        /// <param name="assetId">The id of the asset to archive.</param>
        /// <param name="version">The last known version of the asset.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.ManagementAsset"/> archived.</returns>
        Task<ManagementAsset> ArchiveAssetAsync(string assetId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Archives an entry by the specified id.
        /// </summary>
        /// <param name="entryId">The id of the entry.</param>
        /// <param name="version">The last known version of the entry.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into <see cref="Entry{dynamic}"/></returns>
        Task<Entry<dynamic>> ArchiveEntryAsync(string entryId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates an <see cref="Contentful.Core.Models.Management.ApiKey"/> in a space.
        /// </summary>
        /// <param name="name">The name of the API key to create.</param>
        /// <param name="description">The description of the API key to create.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.ApiKey"/>.</returns>
        Task<ApiKey> CreateApiKeyAsync(string name, string description, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a locale in the specified <see cref="Space"/>.
        /// </summary>
        /// <param name="locale">The <see cref="Contentful.Core.Models.Management.Locale"/> to create.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.Locale"/>.</returns>
        Task<Locale> CreateLocaleAsync(Locale locale, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates or updates an <see cref="Contentful.Core.Models.Management.ManagementAsset"/>. Updates if an asset with the same id already exists.
        /// </summary>
        /// <param name="asset">The asset to create or update.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="version">The last known version of the entry. Must be set when updating an asset.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The updated <see cref="Contentful.Core.Models.Management.ManagementAsset"/></returns>
        Task<ManagementAsset> CreateOrUpdateAssetAsync(ManagementAsset asset, string spaceId = null, int? version = default(int?), CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates or updates a ContentType. Updates if a content type with the same id already exists.
        /// </summary>
        /// <param name="contentType">The <see cref="ContentType"/> to create or update. **Remember to set the id property.**</param>
        /// <param name="spaceId">The id of the space to create the content type in. Will default to the one set when creating the client.</param>
        /// <param name="version">The last version known of the content type. Must be set for existing content types. Should be null if one is created.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created or updated <see cref="ContentType"/>.</returns>
        Task<ContentType> CreateOrUpdateContentTypeAsync(ContentType contentType, string spaceId = null, int? version = default(int?), CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates an <see cref="Entry{T}"/>.
        /// </summary>
        /// <param name="entry">The entry to create or update.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="contentTypeId">The id of the <see cref="ContentType"/> of the entry.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Entry{T}"/>.</returns>
        Task<Entry<dynamic>> CreateEntryAsync(Entry<dynamic> entry, string contentTypeId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates or updates an <see cref="Entry{T}"/>. Updates if an entry with the same id already exists.
        /// </summary>
        /// <param name="entry">The entry to create or update.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="contentTypeId">The id of the <see cref="ContentType"/> of the entry. Need only be set if you are creating a new entry.</param>
        /// <param name="version">The last known version of the entry. Must be set when updating an entry.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created or updated <see cref="Entry{T}"/>.</returns>
        Task<Entry<dynamic>> CreateOrUpdateEntryAsync(Entry<dynamic> entry, string spaceId = null, string contentTypeId = null, int? version = default(int?), CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates or updates a webhook in a <see cref="Space"/>.  Updates if a webhook with the same id already exists.
        /// </summary>
        /// <param name="webhook">The webhook to create or update.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.WebHook"/>.</returns>
        Task<WebHook> CreateOrUpdateWebHookAsync(WebHook webhook, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a role in a <see cref="Space"/>.
        /// </summary>
        /// <param name="role">The role to create.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.Role"/>.</returns>
        Task<Role> CreateRoleAsync(Role role, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a new space in Contentful.
        /// </summary>
        /// <param name="name">The name of the space to create.</param>
        /// <param name="defaultLocale">The default locale for this space.</param>
        /// <param name="organisation">The organisation to create a space for. Not required if the account belongs to only one organisation.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Space"/></returns>
        Task<Space> CreateSpaceAsync(string name, string defaultLocale, string organisation = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a membership in a <see cref="Space"/>.
        /// </summary>
        /// <param name="spaceMembership">The membership to create.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.SpaceMembership"/>.</returns>
        Task<SpaceMembership> CreateSpaceMembershipAsync(SpaceMembership spaceMembership, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a webhook in a <see cref="Space"/>.
        /// </summary>
        /// <param name="webhook">The webhook to create.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.WebHook"/>.</returns>
        Task<WebHook> CreateWebHookAsync(WebHook webhook, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deactivates a <see cref="ContentType"/> by the specified id.
        /// </summary>
        /// <param name="contentTypeId">The id of the content type.</param>
        /// <param name="spaceId">The id of the space to deactivate the content type in. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into a <see cref="ContentType"/>.</returns>
        Task DeactivateContentTypeAsync(string contentTypeId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes an asset by the specified id.
        /// </summary>
        /// <param name="assetId">The id of the asset to delete.</param>
        /// <param name="version">The last known version of the asset.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        Task DeleteAssetAsync(string assetId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes a <see cref="ContentType"/> by the specified id.
        /// </summary>
        /// <param name="contentTypeId">The id of the content type.</param>
        /// <param name="spaceId">The id of the space to delete the content type in. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into a <see cref="ContentType"/>.</returns>
        Task DeleteContentTypeAsync(string contentTypeId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes a single entry by the specified id.
        /// </summary>
        /// <param name="entryId">The id of the entry.</param>
        /// <param name="version">The last known version of the entry.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        Task DeleteEntryAsync(string entryId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes a locale by the specified id.
        /// </summary>
        /// <param name="localeId">The id of the locale to delete.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        Task DeleteLocaleAsync(string localeId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes a role by the specified id.
        /// </summary>
        /// <param name="roleId">The id of the role to delete.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        Task DeleteRoleAsync(string roleId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes a space in Contentful.
        /// </summary>
        /// <param name="id">The id of the space to delete.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns></returns>
        Task DeleteSpaceAsync(string id, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes a <see cref="Contentful.Core.Models.Management.SpaceMembership"/> for a space.
        /// </summary>
        /// <param name="spaceMembershipId">The id of the space membership to delete.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        Task DeleteSpaceMembershipAsync(string spaceMembershipId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes a webhook from a <see cref="Space"/>.
        /// </summary>
        /// <param name="webhookId">The id of the webhook to delete.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        Task DeleteWebHookAsync(string webhookId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get all activated content types of a space.
        /// </summary>
        /// <param name="spaceId">The id of the space to get the activated content types of. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ContentType"/>.</returns>
        Task<IEnumerable<ContentType>> GetActivatedContentTypesAsync(string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a collection of all <see cref="Contentful.Core.Models.Management.ApiKey"/> in a space.
        /// </summary>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.ApiKey"/>.</returns>
        Task<ContentfulCollection<ApiKey>> GetAllApiKeysAsync(string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets all <see cref="Contentful.Core.Models.Management.Role">roles</see> of a space.
        /// </summary>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.Role"/>.</returns>
        Task<ContentfulCollection<Role>> GetAllRolesAsync(string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets all snapsohts for an <see cref="Entry{T}"/>.
        /// </summary>
        /// <param name="entryId">The id of the entry to get snapshots for.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A collection of <see cref="Contentful.Core.Models.Management.Snapshot"/>.</returns>
        Task<ContentfulCollection<Snapshot>> GetAllSnapshotsForEntryAsync(string entryId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets an asset by the specified id.
        /// </summary>
        /// <param name="assetId">The id of the asset to get.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.ManagementAsset"/>.</returns>
        Task<ManagementAsset> GetAssetAsync(string assetId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets all assets of a space, filtered by an optional <see cref="QueryBuilder{T}"/>.
        /// </summary>
        /// <param name="queryBuilder">The optional <see cref="QueryBuilder{T}"/> to add additional filtering to the query.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.ManagementAsset"/>.</returns>
        /// <exception cref="Contentful.Core.Errors.ContentfulException">There was an error when communicating with the Contentful API.</exception>
        Task<ContentfulCollection<ManagementAsset>> GetAssetsCollectionAsync(QueryBuilder<Asset> queryBuilder, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));


        /// <summary>
        /// Gets all assets in the space.
        /// </summary>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.ManagementAsset"/>.</returns>
        Task<ContentfulCollection<ManagementAsset>> GetAssetsCollectionAsync(string queryString, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a <see cref="ContentType"/> by the specified id.
        /// </summary>
        /// <param name="contentTypeId">The id of the content type.</param>
        /// <param name="spaceId">The id of the space to get the content type from. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into a <see cref="ContentType"/>.</returns>
        Task<ContentType> GetContentTypeAsync(string contentTypeId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get all content types of a space.
        /// </summary>
        /// <param name="spaceId">The id of the space to get the content types of. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ContentType"/>.</returns>
        Task<IEnumerable<ContentType>> GetContentTypesAsync(string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a <see cref="Contentful.Core.Models.Management.EditorInterface"/> for a specific <seealso cref="ContentType"/>.
        /// </summary>
        /// <param name="contentTypeId">The id of the content type.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into a <see cref="Contentful.Core.Models.Management.EditorInterface"/>.</returns>
        Task<EditorInterface> GetEditorInterfaceAsync(string contentTypeId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets all the entries of a space, filtered by an optional <see cref="QueryBuilder{T}"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="IContentfulResource"/> to serialize the response into.</typeparam>
        /// <param name="queryBuilder">The optional <see cref="QueryBuilder{T}"/> to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of items.</returns>
        Task<ContentfulCollection<T>> GetEntriesCollectionAsync<T>(QueryBuilder<T> queryBuilder, CancellationToken cancellationToken = default(CancellationToken)) where T : IContentfulResource;

        /// <summary>
        /// Gets all the entries of a space, filtered by an optional querystring. A simpler approach than 
        /// to construct a query manually is to use the <see cref="QueryBuilder{T}"/> class.
        /// </summary>
        /// <typeparam name="T">The <see cref="IContentfulResource"/> to serialize the response into.</typeparam>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of items.</returns>
        Task<ContentfulCollection<T>> GetEntriesCollectionAsync<T>(string queryString = null, CancellationToken cancellationToken = default(CancellationToken)) where T : IContentfulResource;

        /// <summary>
        /// Get a single entry by the specified id.
        /// </summary>
        /// <param name="entryId">The id of the entry.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into <see cref="Entry{dynamic}"/></returns>
        Task<Entry<dynamic>> GetEntryAsync(string entryId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a locale in the specified <see cref="Space"/>.
        /// </summary>
        /// <param name="localeId">The id of the locale to get.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The requested <see cref="Contentful.Core.Models.Management.Locale"/>.</returns>
        Task<Locale> GetLocaleAsync(string localeId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets all locales in a <see cref="Space"/>.
        /// </summary>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{Locale}"/> of locales.</returns>
        Task<ContentfulCollection<Locale>> GetLocalesCollectionAsync(string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets all published assets in the space.
        /// </summary>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.ManagementAsset"/>.</returns>
        Task<ContentfulCollection<ManagementAsset>> GetPublishedAssetsCollectionAsync(string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a role by the specified id.
        /// </summary>
        /// <param name="roleId">The id of the role.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.Role"/></returns>
        Task<Role> GetRoleAsync(string roleId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a single snapshot for an <see cref="Entry{T}"/>
        /// </summary>
        /// <param name="snapshotId">The id of the snapshot to get.</param>
        /// <param name="entryId">The id of entry the snapshot belongs to.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.Snapshot"/>.</returns>
        Task<Snapshot> GetSnapshotForEntryAsync(string snapshotId, string entryId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a space in Contentful.
        /// </summary>
        /// <param name="id">The id of the space to get.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Space" /></returns>
        Task<Space> GetSpaceAsync(string id, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a single <see cref="Contentful.Core.Models.Management.SpaceMembership"/> for a space.
        /// </summary>
        /// <param name="spaceMembershipId">The id of the space membership to get.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.SpaceMembership"/>.</returns>
        Task<SpaceMembership> GetSpaceMembershipAsync(string spaceMembershipId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a collection of <see cref="Contentful.Core.Models.Management.SpaceMembership"/> for the user.
        /// </summary>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A collection of <see cref="Contentful.Core.Models.Management.SpaceMembership"/>.</returns>
        Task<ContentfulCollection<SpaceMembership>> GetSpaceMembershipsAsync(string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets all spaces in Contentful.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Space"/>.</returns>
        Task<IEnumerable<Space>> GetSpacesAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a single webhook from a <see cref="Space"/>.
        /// </summary>
        /// <param name="webhookId">The id of the webhook to get.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.WebHook"/>.</returns>
        Task<WebHook> GetWebHookAsync(string webhookId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets the details of a specific webhook call.
        /// </summary>
        /// <param name="callId">The id of the call to get details for.</param>
        /// <param name="webhookId">The id of the webhook to get details for.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.WebHookCallDetails"/>.</returns>
        Task<WebHookCallDetails> GetWebHookCallDetailsAsync(string callId, string webhookId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets all recent call details for a webhook.
        /// </summary>
        /// <param name="webhookId">The id of the webhook to get details for.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.WebHookCallDetails"/>.</returns>
        Task<ContentfulCollection<WebHookCallDetails>> GetWebHookCallDetailsCollectionAsync(string webhookId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a response containing an overview of the recent webhook calls.
        /// </summary>
        /// <param name="webhookId">The id of the webhook to get health details for.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="Contentful.Core.Models.Management.WebHookHealthResponse"/>.</returns>
        Task<WebHookHealthResponse> GetWebHookHealthAsync(string webhookId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets all webhooks for a <see cref="Space"/>.
        /// </summary>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.WebHook"/>.</returns>
        Task<ContentfulCollection<WebHook>> GetWebHooksCollectionAsync(string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Processes an asset by the specified id.
        /// </summary>
        /// <param name="assetId">The id of the asset to process.</param>
        /// <param name="version">The last known version of the asset.</param>
        /// <param name="locale">The locale for which files should be processed.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        Task ProcessAssetAsync(string assetId, int version, string locale, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Publishes an asset by the specified id.
        /// </summary>
        /// <param name="assetId">The id of the asset to publish.</param>
        /// <param name="version">The last known version of the asset.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.ManagementAsset"/> published.</returns>
        Task<ManagementAsset> PublishAssetAsync(string assetId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Publishes an entry by the specified id.
        /// </summary>
        /// <param name="entryId">The id of the entry.</param>
        /// <param name="version">The last known version of the entry.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into <see cref="Entry{dynamic}"/></returns>
        Task<Entry<dynamic>> PublishEntryAsync(string entryId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Unarchives an asset by the specified id.
        /// </summary>
        /// <param name="assetId">The id of the asset to unarchive.</param>
        /// <param name="version">The last known version of the asset.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.ManagementAsset"/> unarchived.</returns>
        Task<ManagementAsset> UnarchiveAssetAsync(string assetId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Unarchives an entry by the specified id.
        /// </summary>
        /// <param name="entryId">The id of the entry.</param>
        /// <param name="version">The last known version of the entry.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into <see cref="Entry{dynamic}"/></returns>
        Task<Entry<dynamic>> UnarchiveEntryAsync(string entryId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Unpublishes an asset by the specified id.
        /// </summary>
        /// <param name="assetId">The id of the asset to unpublish.</param>
        /// <param name="version">The last known version of the asset.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.ManagementAsset"/> unpublished.</returns>
        Task<ManagementAsset> UnpublishAssetAsync(string assetId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Unpublishes an entry by the specified id.
        /// </summary>
        /// <param name="entryId">The id of the entry.</param>
        /// <param name="version">The last known version of the entry.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into <see cref="Entry{dynamic}"/></returns>
        Task<Entry<dynamic>> UnpublishEntryAsync(string entryId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Updates a <see cref="Contentful.Core.Models.Management.EditorInterface"/> for a specific <see cref="ContentType"/>.
        /// </summary>
        /// <param name="editorInterface">The editor interface to update.</param>
        /// <param name="contentTypeId">The id of the content type.</param>
        /// <param name="version">The last known version of the content type.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into a <see cref="Contentful.Core.Models.Management.EditorInterface"/>.</returns>
        Task<EditorInterface> UpdateEditorInterfaceAsync(EditorInterface editorInterface, string contentTypeId, int version, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Updates a locale in the specified <see cref="Space"/>.
        /// </summary>
        /// <param name="locale">The <see cref="Contentful.Core.Models.Management.Locale"/> to update.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.Locale"/>.</returns>
        Task<Locale> UpdateLocaleAsync(Locale locale, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));
        
        /// <summary>
        /// Updates a role in a <see cref="Space"/>.
        /// </summary>
        /// <param name="role">The role to update.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The updated <see cref="Contentful.Core.Models.Management.Role"/>.</returns>
        Task<Role> UpdateRoleAsync(Role role, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Updates a <see cref="Contentful.Core.Models.Management.SpaceMembership"/> for a space.
        /// </summary>
        /// <param name="spaceMembership">The membership to update.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.SpaceMembership"/>.</returns>
        Task<SpaceMembership> UpdateSpaceMembershipAsync(SpaceMembership spaceMembership, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Updates the name of a space in Contentful.
        /// </summary>
        /// <param name="space">The space to update, needs to contain at minimum name, Id and version.</param>
        /// <param name="organisation">The organisation to update a space for. Not required if the account belongs to only one organisation.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The updated <see cref="Space"/></returns>
        Task<Space> UpdateSpaceNameAsync(Space space, string organisation = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Updates a space in Contentful.
        /// </summary>
        /// <param name="id">The id of the space to update.</param>
        /// <param name="name">The name to update to.</param>
        /// <param name="version">The version of the space that will be updated.</param>
        /// <param name="organisation">The organisation to update a space for. Not required if the account belongs to only one organisation.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The updated <see cref="Space"/></returns>
        Task<Space> UpdateSpaceNameAsync(string id, string name, int version, string organisation = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets an upload <see cref="SystemProperties"/> by the specified id.
        /// </summary>
        /// <param name="uploadId">The id of the uploaded file.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="SystemProperties"/> with metadata of the upload.</returns>
        Task<UploadReference> GetUploadAsync(string uploadId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Uploads the specified bytes to Contentful.
        /// </summary>
        /// <param name="bytes">The bytes to upload.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="SystemProperties"/> with an id of the created upload.</returns>
        Task<UploadReference> UploadFileAsync(byte[] bytes, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets an upload <see cref="SystemProperties"/> by the specified id.
        /// </summary>
        /// <param name="uploadId">The id of the uploaded file.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="SystemProperties"/> with metadata of the upload.</returns>
        Task DeleteUploadAsync(string uploadId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Uploads an array of bytes and creates an asset in Contentful as well as processing that asset.
        /// </summary>
        /// <param name="asset">The asset to create</param>
        /// <param name="bytes">The bytes to upload.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.ManagementAsset"/>.</returns>
        Task<ManagementAsset> UploadFileAndCreateAssetAsync(ManagementAsset asset, byte[] bytes, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a collection of all <see cref="Contentful.Core.Models.Management.UiExtension"/> for a space.
        /// </summary>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.UiExtension"/>.</returns>
        Task<ContentfulCollection<UiExtension>> GetAllExtensionsAsync(string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a UiExtension in a <see cref="Space"/>.
        /// </summary>
        /// <param name="extension">The UI extension to create.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.UiExtension"/>.</returns>
        Task<UiExtension> CreateExtensionAsync(UiExtension extension, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates or updates a UI extension. Updates if an extension with the same id already exists.
        /// </summary>
        /// <param name="extension">The <see cref="Contentful.Core.Models.Management.UiExtension"/> to create or update. **Remember to set the id property.**</param>
        /// <param name="spaceId">The id of the space to create the content type in. Will default to the one set when creating the client.</param>
        /// <param name="version">The last version known of the extension. Must be set for existing extensions. Should be null if one is created.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created or updated <see cref="Contentful.Core.Models.Management.UiExtension"/>.</returns>
        Task<UiExtension> CreateOrUpdateExtensionAsync(UiExtension extension, string spaceId = null, int? version = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a single <see cref="Contentful.Core.Models.Management.UiExtension"/> for a space.
        /// </summary>
        /// <param name="extensionId">The id of the extension to get.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.User"/>.</returns>
        Task<UiExtension> GetExtensionAsync(string extensionId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes a <see cref="Contentful.Core.Models.Management.UiExtension"/> by the specified id.
        /// </summary>
        /// <param name="extensionId">The id of the extension.</param>
        /// <param name="spaceId">The id of the space to delete the extension in. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        Task DeleteExtensionAsync(string extensionId, string spaceId = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a CMA management token that can be used to access the Contentful Management API.
        /// </summary>
        /// <param name="token">The token to create.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.ManagementToken"/>.</returns>
        Task<ManagementToken> CreateManagementTokenAsync(ManagementToken token, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a collection of all <see cref="Contentful.Core.Models.Management.ManagementToken"/> for a user. **Note that the actual token will not be part of the response. 
        /// It is only available directly after creation of a token for security reasons.**
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.UiExtension"/>.</returns>
        Task<ContentfulCollection<ManagementToken>> GetAllManagementTokensAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a single <see cref="Contentful.Core.Models.Management.ManagementToken"/> for a user. **Note that the actual token will not be part of the response. 
        /// It is only available directly after creation of a token for security reasons.**
        /// </summary>
        /// <param name="managementTokenId">The id of the management token to get.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.ManagementToken"/>.</returns>
        Task<ManagementToken> GetManagementTokenAsync(string managementTokenId, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Revokes a single <see cref="Contentful.Core.Models.Management.ManagementToken"/> for a user.
        /// </summary>
        /// <param name="managementTokenId">The id of the management token to revoke.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The revoked <see cref="Contentful.Core.Models.Management.ManagementToken"/>.</returns>
        Task<ManagementToken> RevokeManagementTokenAsync(string managementTokenId, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a single <see cref="Contentful.Core.Models.Management.User"/> for the currently logged in user.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.User"/>.</returns>
        Task<User> GetCurrentUserAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a collection of all <see cref="Contentful.Core.Models.Management.Organization"/> for a user.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.Organization"/>.</returns>
        /// <exception cref="ContentfulException">There was an error when communicating with the Contentful API.</exception>
        Task<ContentfulCollection<Organization>> GetOrganizationsAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}