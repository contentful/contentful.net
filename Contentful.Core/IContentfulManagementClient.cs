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
        Task<ContentType> ActivateContentType(string contentTypeId, int version, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Archives an asset by the specified id.
        /// </summary>
        /// <param name="assetId">The id of the asset to archive.</param>
        /// <param name="version">The last known version of the asset.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.ManagementAsset"/> archived.</returns>
        Task<ManagementAsset> ArchiveAsset(string assetId, int version, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Archives an entry by the specified id.
        /// </summary>
        /// <param name="entryId">The id of the entry.</param>
        /// <param name="version">The last known version of the entry.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into <see cref="Entry{dynamic}"/></returns>
        Task<Entry<dynamic>> ArchiveEntry(string entryId, int version, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates an <see cref="Contentful.Core.Models.Management.ApiKey"/> in a space.
        /// </summary>
        /// <param name="name">The name of the API key to create.</param>
        /// <param name="description">The description of the API key to create.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.ApiKey"/>.</returns>
        Task<ApiKey> CreateApiKey(string name, string description, string spaceId = null, CancellationToken cancellationToken = default);

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
        /// <exception cref="Contentful.Core.Errors.ContentfulException">There was an error when communicating with the Contentful API.</exception>
        Task<ApiKey> UpdateApiKey(string id, string name, string description, int version, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a locale in the specified <see cref="Space"/>.
        /// </summary>
        /// <param name="locale">The <see cref="Contentful.Core.Models.Management.Locale"/> to create.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.Locale"/>.</returns>
        Task<Locale> CreateLocale(Locale locale, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates or updates an <see cref="Contentful.Core.Models.Management.ManagementAsset"/>. Updates if an asset with the same id already exists.
        /// </summary>
        /// <param name="asset">The asset to create or update.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="version">The last known version of the entry. Must be set when updating an asset.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The updated <see cref="Contentful.Core.Models.Management.ManagementAsset"/></returns>
        Task<ManagementAsset> CreateOrUpdateAsset(ManagementAsset asset, string spaceId = null, int? version = default, CancellationToken cancellationToken = default);


        /// <summary>
        /// Creates an <see cref="Contentful.Core.Models.Management.ManagementAsset"/> with a randomly created id.
        /// </summary>
        /// <param name="asset">The asset to create.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.ManagementAsset"/></returns>
        Task<ManagementAsset> CreateAsset(ManagementAsset asset, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates or updates a ContentType. Updates if a content type with the same id already exists.
        /// </summary>
        /// <param name="contentType">The <see cref="ContentType"/> to create or update. **Remember to set the id property.**</param>
        /// <param name="spaceId">The id of the space to create the content type in. Will default to the one set when creating the client.</param>
        /// <param name="version">The last version known of the content type. Must be set for existing content types. Should be null if one is created.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created or updated <see cref="ContentType"/>.</returns>
        Task<ContentType> CreateOrUpdateContentType(ContentType contentType, string spaceId = null, int? version = default, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates an <see cref="Entry{T}"/>.
        /// </summary>
        /// <param name="entry">The entry to create or update.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="contentTypeId">The id of the <see cref="ContentType"/> of the entry.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Entry{T}"/>.</returns>
        Task<Entry<dynamic>> CreateEntry(Entry<dynamic> entry, string contentTypeId, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates an entry.
        /// </summary>
        /// <param name="entry">The object to create an entry from.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="contentTypeId">The id of the <see cref="ContentType"/> of the entry.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created entry.</returns>
        Task<T> CreateEntry<T>(T entry, string contentTypeId, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates or updates an <see cref="Entry{T}"/>. Updates if an entry with the same id already exists.
        /// </summary>
        /// <param name="entry">The entry to create or update.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="contentTypeId">The id of the <see cref="ContentType"/> of the entry. Need only be set if you are creating a new entry.</param>
        /// <param name="version">The last known version of the entry. Must be set when updating an entry.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created or updated <see cref="Entry{T}"/>.</returns>
        Task<Entry<dynamic>> CreateOrUpdateEntry(Entry<dynamic> entry, string spaceId = null, string contentTypeId = null, int? version = default, CancellationToken cancellationToken = default);

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
        Task<T> CreateOrUpdateEntry<T>(T entry, string id, string spaceId = null, string contentTypeId = null, int? version = null, CancellationToken cancellationToken = default);

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
        Task<Entry<dynamic>> CreateEntryForLocale(object entry, string id, string contentTypeId, string locale = null, string spaceId = null, CancellationToken cancellationToken = default);
        

        /// <summary>
        /// Updates an entry fields for a certain locale using the values from the provided object.
        /// </summary>
        /// <param name="entry">The object to use as values for the entry fields.</param>
        /// <param name="id">The id of the entry to update.</param>
        /// <param name="locale">The locale to set the fields for. The default locale for the space will be used if this parameter is null.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created or updated <see cref="Entry{T}"/>.</returns>
        Task<Entry<dynamic>> UpdateEntryForLocale(object entry, string id, string locale = null, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates or updates a webhook in a <see cref="Space"/>.  Updates if a webhook with the same id already exists.
        /// </summary>
        /// <param name="webhook">The webhook to create or update.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="version">The last known version of the webhook.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.Webhook"/>.</returns>
        Task<Webhook> CreateOrUpdateWebhook(Webhook webhook, string spaceId = null, int? version = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a role in a <see cref="Space"/>.
        /// </summary>
        /// <param name="role">The role to create.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.Role"/>.</returns>
        Task<Role> CreateRole(Role role, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new space in Contentful.
        /// </summary>
        /// <param name="name">The name of the space to create.</param>
        /// <param name="defaultLocale">The default locale for this space.</param>
        /// <param name="organisation">The organisation to create a space for. Not required if the account belongs to only one organisation.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Space"/></returns>
        Task<Space> CreateSpace(string name, string defaultLocale, string organisation = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a membership in a <see cref="Space"/>.
        /// </summary>
        /// <param name="spaceMembership">The membership to create.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.SpaceMembership"/>.</returns>
        Task<SpaceMembership> CreateSpaceMembership(SpaceMembership spaceMembership, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a webhook in a <see cref="Space"/>.
        /// </summary>
        /// <param name="webhook">The webhook to create.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.Webhook"/>.</returns>
        Task<Webhook> CreateWebhook(Webhook webhook, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deactivates a <see cref="ContentType"/> by the specified id.
        /// </summary>
        /// <param name="contentTypeId">The id of the content type.</param>
        /// <param name="spaceId">The id of the space to deactivate the content type in. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into a <see cref="ContentType"/>.</returns>
        Task DeactivateContentType(string contentTypeId, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an asset by the specified id.
        /// </summary>
        /// <param name="assetId">The id of the asset to delete.</param>
        /// <param name="version">The last known version of the asset.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        Task DeleteAsset(string assetId, int version, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a <see cref="ContentType"/> by the specified id.
        /// </summary>
        /// <param name="contentTypeId">The id of the content type.</param>
        /// <param name="spaceId">The id of the space to delete the content type in. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into a <see cref="ContentType"/>.</returns>
        Task DeleteContentType(string contentTypeId, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a single entry by the specified id.
        /// </summary>
        /// <param name="entryId">The id of the entry.</param>
        /// <param name="version">The last known version of the entry.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        Task DeleteEntry(string entryId, int version, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a locale by the specified id.
        /// </summary>
        /// <param name="localeId">The id of the locale to delete.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        Task DeleteLocale(string localeId, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a role by the specified id.
        /// </summary>
        /// <param name="roleId">The id of the role to delete.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        Task DeleteRole(string roleId, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a space in Contentful.
        /// </summary>
        /// <param name="id">The id of the space to delete.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns></returns>
        Task DeleteSpace(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a <see cref="Contentful.Core.Models.Management.SpaceMembership"/> for a space.
        /// </summary>
        /// <param name="spaceMembershipId">The id of the space membership to delete.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        Task DeleteSpaceMembership(string spaceMembershipId, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a webhook from a <see cref="Space"/>.
        /// </summary>
        /// <param name="webhookId">The id of the webhook to delete.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        Task DeleteWebhook(string webhookId, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all activated content types of a space.
        /// </summary>
        /// <param name="spaceId">The id of the space to get the activated content types of. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ContentType"/>.</returns>
        Task<IEnumerable<ContentType>> GetActivatedContentTypes(string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all activated content types of a space.
        /// </summary>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <param name="spaceId">The id of the space to get the activated content types of. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ContentType"/>.</returns>
        Task<IEnumerable<ContentType>> GetActivatedContentTypes(string queryString, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a collection of all <see cref="Contentful.Core.Models.Management.ApiKey"/> in a space.
        /// </summary>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.ApiKey"/>.</returns>
        Task<ContentfulCollection<ApiKey>> GetAllApiKeys(string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a collection of all preview <see cref="Contentful.Core.Models.Management.ApiKey"/> in a space.
        /// </summary>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.ApiKey"/>.</returns>
        /// <exception cref="Contentful.Core.Errors.ContentfulException">There was an error when communicating with the Contentful API.</exception>
        Task<ContentfulCollection<ApiKey>> GetAllPreviewApiKeys(string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets an <see cref="Contentful.Core.Models.Management.ApiKey"/> in a space.
        /// </summary>
        /// <param name="apiKeyId">The id of the api key get.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.ApiKey"/>.</returns>
        /// <exception cref="Contentful.Core.Errors.ContentfulException">There was an error when communicating with the Contentful API.</exception>
        Task<ApiKey> GetApiKey(string apiKeyId, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a preview <see cref="Contentful.Core.Models.Management.ApiKey"/> in a space.
        /// </summary>
        /// <param name="apiKeyId">The id of the api key get.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.ApiKey"/>.</returns>
        /// <exception cref="Contentful.Core.Errors.ContentfulException">There was an error when communicating with the Contentful API.</exception>
        Task<ApiKey> GetPreviewApiKey(string apiKeyId, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an api key by the specified id.
        /// </summary>
        /// <param name="apiKeyId">The id of the api key to delete.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="SystemProperties"/> with metadata of the upload.</returns>
        Task DeleteApiKey(string apiKeyId, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all <see cref="Contentful.Core.Models.Management.Role">roles</see> of a space.
        /// </summary>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.Role"/>.</returns>
        Task<ContentfulCollection<Role>> GetAllRoles(string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all snapsohts for an <see cref="Entry{T}"/>.
        /// </summary>
        /// <param name="entryId">The id of the entry to get snapshots for.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A collection of <see cref="Contentful.Core.Models.Management.Snapshot"/>.</returns>
        Task<ContentfulCollection<Snapshot>> GetAllSnapshotsForEntry(string entryId, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets an asset by the specified id.
        /// </summary>
        /// <param name="assetId">The id of the asset to get.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.ManagementAsset"/>.</returns>
        Task<ManagementAsset> GetAsset(string assetId, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all assets of a space, filtered by an optional <see cref="QueryBuilder{T}"/>.
        /// </summary>
        /// <param name="queryBuilder">The optional <see cref="QueryBuilder{T}"/> to add additional filtering to the query.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.ManagementAsset"/>.</returns>
        /// <exception cref="Contentful.Core.Errors.ContentfulException">There was an error when communicating with the Contentful API.</exception>
        Task<ContentfulCollection<ManagementAsset>> GetAssetsCollection(QueryBuilder<Asset> queryBuilder, string spaceId = null, CancellationToken cancellationToken = default);


        /// <summary>
        /// Gets all assets in the space.
        /// </summary>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.ManagementAsset"/>.</returns>
        Task<ContentfulCollection<ManagementAsset>> GetAssetsCollection(string queryString, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a <see cref="ContentType"/> by the specified id.
        /// </summary>
        /// <param name="contentTypeId">The id of the content type.</param>
        /// <param name="spaceId">The id of the space to get the content type from. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into a <see cref="ContentType"/>.</returns>
        Task<ContentType> GetContentType(string contentTypeId, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all content types of a space.
        /// </summary>
        /// <param name="spaceId">The id of the space to get the content types of. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ContentType"/>.</returns>
        Task<IEnumerable<ContentType>> GetContentTypes(string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all content types of a space.
        /// </summary>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <param name="spaceId">The id of the space to get the content types of. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ContentType"/>.</returns>
        Task<IEnumerable<ContentType>> GetContentTypes(string queryString, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a <see cref="Contentful.Core.Models.Management.EditorInterface"/> for a specific <seealso cref="ContentType"/>.
        /// </summary>
        /// <param name="contentTypeId">The id of the content type.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into a <see cref="Contentful.Core.Models.Management.EditorInterface"/>.</returns>
        Task<EditorInterface> GetEditorInterface(string contentTypeId, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all the entries of a space in a specific locale, filtered by an optional <see cref="QueryBuilder{T}"/>.
        /// </summary>
        /// <param name="queryBuilder">The optional <see cref="QueryBuilder{T}"/> to add additional filtering to the query.</param>
        /// <param name="locale">The locale to fetch entries for. Defaults to the default of the space.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of items.</returns>
        Task<ContentfulCollection<T>> GetEntriesForLocale<T>(QueryBuilder<T> queryBuilder, string locale = null, string spaceId = null, CancellationToken cancellationToken = default);
        

            /// <summary>
            /// Gets all the entries of a space, filtered by an optional <see cref="QueryBuilder{T}"/>.
            /// </summary>
            /// <typeparam name="T">The <see cref="IContentfulResource"/> to serialize the response into.</typeparam>
            /// <param name="queryBuilder">The optional <see cref="QueryBuilder{T}"/> to add additional filtering to the query.</param>
            /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
            /// <returns>A <see cref="ContentfulCollection{T}"/> of items.</returns>
            Task<ContentfulCollection<T>> GetEntriesCollection<T>(QueryBuilder<T> queryBuilder, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all the entries of a space, filtered by an optional querystring. A simpler approach than 
        /// to construct a query manually is to use the <see cref="QueryBuilder{T}"/> class.
        /// </summary>
        /// <typeparam name="T">The <see cref="IContentfulResource"/> to serialize the response into.</typeparam>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of items.</returns>
        Task<ContentfulCollection<T>> GetEntriesCollection<T>(string queryString = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get a single entry by the specified id.
        /// </summary>
        /// <param name="entryId">The id of the entry.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into <see cref="Entry{dynamic}"/></returns>
        Task<Entry<dynamic>> GetEntry(string entryId, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a locale in the specified <see cref="Space"/>.
        /// </summary>
        /// <param name="localeId">The id of the locale to get.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The requested <see cref="Contentful.Core.Models.Management.Locale"/>.</returns>
        Task<Locale> GetLocale(string localeId, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all locales in a <see cref="Space"/>.
        /// </summary>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{Locale}"/> of locales.</returns>
        Task<ContentfulCollection<Locale>> GetLocalesCollection(string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all published assets in the space.
        /// </summary>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.ManagementAsset"/>.</returns>
        Task<ContentfulCollection<ManagementAsset>> GetPublishedAssetsCollection(string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a role by the specified id.
        /// </summary>
        /// <param name="roleId">The id of the role.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.Role"/></returns>
        Task<Role> GetRole(string roleId, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a single snapshot for an <see cref="Entry{T}"/>
        /// </summary>
        /// <param name="snapshotId">The id of the snapshot to get.</param>
        /// <param name="entryId">The id of entry the snapshot belongs to.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.Snapshot"/>.</returns>
        Task<Snapshot> GetSnapshotForEntry(string snapshotId, string entryId, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all snapshots for a <see cref="ContentType"/>.
        /// </summary>
        /// <param name="contentTypeId">The id of the content type to get snapshots for.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A collection of <see cref="Contentful.Core.Models.Management.SnapshotContentType"/>.</returns>
        Task<ContentfulCollection<SnapshotContentType>> GetAllSnapshotsForContentType(string contentTypeId, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a single snapshot for a <see cref="ContentType"/>
        /// </summary>
        /// <param name="snapshotId">The id of the snapshot to get.</param>
        /// <param name="contentTypeId">The id of content type the snapshot belongs to.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.SnapshotContentType"/>.</returns>
        Task<SnapshotContentType> GetSnapshotForContentType(string snapshotId, string contentTypeId, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a space in Contentful.
        /// </summary>
        /// <param name="id">The id of the space to get.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Space" /></returns>
        Task<Space> GetSpace(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a single <see cref="Contentful.Core.Models.Management.SpaceMembership"/> for a space.
        /// </summary>
        /// <param name="spaceMembershipId">The id of the space membership to get.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.SpaceMembership"/>.</returns>
        Task<SpaceMembership> GetSpaceMembership(string spaceMembershipId, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a collection of <see cref="Contentful.Core.Models.Management.SpaceMembership"/> for the user.
        /// </summary>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A collection of <see cref="Contentful.Core.Models.Management.SpaceMembership"/>.</returns>
        Task<ContentfulCollection<SpaceMembership>> GetSpaceMemberships(string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all spaces in Contentful.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Space"/>.</returns>
        Task<IEnumerable<Space>> GetSpaces(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a single webhook from a <see cref="Space"/>.
        /// </summary>
        /// <param name="webhookId">The id of the webhook to get.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.Webhook"/>.</returns>
        Task<Webhook> GetWebhook(string webhookId, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the details of a specific webhook call.
        /// </summary>
        /// <param name="callId">The id of the call to get details for.</param>
        /// <param name="webhookId">The id of the webhook to get details for.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.WebhookCallDetails"/>.</returns>
        Task<WebhookCallDetails> GetWebhookCallDetails(string callId, string webhookId, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all recent call details for a webhook.
        /// </summary>
        /// <param name="webhookId">The id of the webhook to get details for.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.WebhookCallDetails"/>.</returns>
        Task<ContentfulCollection<WebhookCallDetails>> GetWebhookCallDetailsCollection(string webhookId, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a response containing an overview of the recent webhook calls.
        /// </summary>
        /// <param name="webhookId">The id of the webhook to get health details for.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="Contentful.Core.Models.Management.WebhookHealthResponse"/>.</returns>
        Task<WebhookHealthResponse> GetWebhookHealth(string webhookId, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all webhooks for a <see cref="Space"/>.
        /// </summary>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.Webhook"/>.</returns>
        Task<ContentfulCollection<Webhook>> GetWebhooksCollection(string spaceId = null, CancellationToken cancellationToken = default);

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
        /// <exception cref="System.ArgumentException">The <see name="assetId">assetId</see> parameter was null or empty.</exception>
        /// <exception cref="Contentful.Core.Errors.ContentfulException">There was an error when communicating with the Contentful API.</exception>
        /// <exception cref="System.TimeoutException">The processing of the asset did not finish within the allotted time.</exception>
        Task<ManagementAsset> ProcessAssetUntilCompleted(string assetId, int version, string locale, int maxDelay = 2000, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Processes an asset by the specified id.
        /// </summary>
        /// <param name="assetId">The id of the asset to process.</param>
        /// <param name="version">The last known version of the asset.</param>
        /// <param name="locale">The locale for which files should be processed.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        Task ProcessAsset(string assetId, int version, string locale, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Publishes an asset by the specified id.
        /// </summary>
        /// <param name="assetId">The id of the asset to publish.</param>
        /// <param name="version">The last known version of the asset.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.ManagementAsset"/> published.</returns>
        Task<ManagementAsset> PublishAsset(string assetId, int version, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Publishes an entry by the specified id.
        /// </summary>
        /// <param name="entryId">The id of the entry.</param>
        /// <param name="version">The last known version of the entry.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into <see cref="Entry{dynamic}"/></returns>
        Task<Entry<dynamic>> PublishEntry(string entryId, int version, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Unarchives an asset by the specified id.
        /// </summary>
        /// <param name="assetId">The id of the asset to unarchive.</param>
        /// <param name="version">The last known version of the asset.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.ManagementAsset"/> unarchived.</returns>
        Task<ManagementAsset> UnarchiveAsset(string assetId, int version, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Unarchives an entry by the specified id.
        /// </summary>
        /// <param name="entryId">The id of the entry.</param>
        /// <param name="version">The last known version of the entry.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into <see cref="Entry{dynamic}"/></returns>
        Task<Entry<dynamic>> UnarchiveEntry(string entryId, int version, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Unpublishes an asset by the specified id.
        /// </summary>
        /// <param name="assetId">The id of the asset to unpublish.</param>
        /// <param name="version">The last known version of the asset.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.ManagementAsset"/> unpublished.</returns>
        Task<ManagementAsset> UnpublishAsset(string assetId, int version, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Unpublishes an entry by the specified id.
        /// </summary>
        /// <param name="entryId">The id of the entry.</param>
        /// <param name="version">The last known version of the entry.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into <see cref="Entry{dynamic}"/></returns>
        Task<Entry<dynamic>> UnpublishEntry(string entryId, int version, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a <see cref="Contentful.Core.Models.Management.EditorInterface"/> for a specific <see cref="ContentType"/>.
        /// </summary>
        /// <param name="editorInterface">The editor interface to update.</param>
        /// <param name="contentTypeId">The id of the content type.</param>
        /// <param name="version">The last known version of the content type.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The response from the API serialized into a <see cref="Contentful.Core.Models.Management.EditorInterface"/>.</returns>
        Task<EditorInterface> UpdateEditorInterface(EditorInterface editorInterface, string contentTypeId, int version, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a locale in the specified <see cref="Space"/>.
        /// </summary>
        /// <param name="locale">The <see cref="Contentful.Core.Models.Management.Locale"/> to update.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.Locale"/>.</returns>
        Task<Locale> UpdateLocale(Locale locale, string spaceId = null, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Updates a role in a <see cref="Space"/>.
        /// </summary>
        /// <param name="role">The role to update.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The updated <see cref="Contentful.Core.Models.Management.Role"/>.</returns>
        Task<Role> UpdateRole(Role role, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a <see cref="Contentful.Core.Models.Management.SpaceMembership"/> for a space.
        /// </summary>
        /// <param name="spaceMembership">The membership to update.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.SpaceMembership"/>.</returns>
        Task<SpaceMembership> UpdateSpaceMembership(SpaceMembership spaceMembership, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the name of a space in Contentful.
        /// </summary>
        /// <param name="space">The space to update, needs to contain at minimum name, Id and version.</param>
        /// <param name="organisation">The organisation to update a space for. Not required if the account belongs to only one organisation.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The updated <see cref="Space"/></returns>
        Task<Space> UpdateSpaceName(Space space, string organisation = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a space in Contentful.
        /// </summary>
        /// <param name="id">The id of the space to update.</param>
        /// <param name="name">The name to update to.</param>
        /// <param name="version">The version of the space that will be updated.</param>
        /// <param name="organisation">The organisation to update a space for. Not required if the account belongs to only one organisation.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The updated <see cref="Space"/></returns>
        Task<Space> UpdateSpaceName(string id, string name, int version, string organisation = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets an upload <see cref="SystemProperties"/> by the specified id.
        /// </summary>
        /// <param name="uploadId">The id of the uploaded file.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="SystemProperties"/> with metadata of the upload.</returns>
        Task<UploadReference> GetUpload(string uploadId, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uploads the specified bytes to Contentful.
        /// </summary>
        /// <param name="bytes">The bytes to upload.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="SystemProperties"/> with an id of the created upload.</returns>
        Task<UploadReference> UploadFile(byte[] bytes, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets an upload <see cref="SystemProperties"/> by the specified id.
        /// </summary>
        /// <param name="uploadId">The id of the uploaded file.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="SystemProperties"/> with metadata of the upload.</returns>
        Task DeleteUpload(string uploadId, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uploads an array of bytes and creates an asset in Contentful as well as processing that asset.
        /// </summary>
        /// <param name="asset">The asset to create</param>
        /// <param name="bytes">The bytes to upload.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.ManagementAsset"/>.</returns>
        Task<ManagementAsset> UploadFileAndCreateAsset(ManagementAsset asset, byte[] bytes, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a collection of all <see cref="Contentful.Core.Models.Management.UiExtension"/> for a space.
        /// </summary>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.UiExtension"/>.</returns>
        Task<ContentfulCollection<UiExtension>> GetAllExtensions(string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a UiExtension in a <see cref="Space"/>.
        /// </summary>
        /// <param name="extension">The UI extension to create.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.UiExtension"/>.</returns>
        Task<UiExtension> CreateExtension(UiExtension extension, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates or updates a UI extension. Updates if an extension with the same id already exists.
        /// </summary>
        /// <param name="extension">The <see cref="Contentful.Core.Models.Management.UiExtension"/> to create or update. **Remember to set the id property.**</param>
        /// <param name="spaceId">The id of the space to create the content type in. Will default to the one set when creating the client.</param>
        /// <param name="version">The last version known of the extension. Must be set for existing extensions. Should be null if one is created.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created or updated <see cref="Contentful.Core.Models.Management.UiExtension"/>.</returns>
        Task<UiExtension> CreateOrUpdateExtension(UiExtension extension, string spaceId = null, int? version = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a single <see cref="Contentful.Core.Models.Management.UiExtension"/> for a space.
        /// </summary>
        /// <param name="extensionId">The id of the extension to get.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.User"/>.</returns>
        Task<UiExtension> GetExtension(string extensionId, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a <see cref="Contentful.Core.Models.Management.UiExtension"/> by the specified id.
        /// </summary>
        /// <param name="extensionId">The id of the extension.</param>
        /// <param name="spaceId">The id of the space to delete the extension in. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        Task DeleteExtension(string extensionId, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a CMA management token that can be used to access the Contentful Management API.
        /// </summary>
        /// <param name="token">The token to create.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.ManagementToken"/>.</returns>
        Task<ManagementToken> CreateManagementToken(ManagementToken token, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a collection of all <see cref="Contentful.Core.Models.Management.ManagementToken"/> for a user. **Note that the actual token will not be part of the response. 
        /// It is only available directly after creation of a token for security reasons.**
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.UiExtension"/>.</returns>
        Task<ContentfulCollection<ManagementToken>> GetAllManagementTokens(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a single <see cref="Contentful.Core.Models.Management.ManagementToken"/> for a user. **Note that the actual token will not be part of the response. 
        /// It is only available directly after creation of a token for security reasons.**
        /// </summary>
        /// <param name="managementTokenId">The id of the management token to get.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.ManagementToken"/>.</returns>
        Task<ManagementToken> GetManagementToken(string managementTokenId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Revokes a single <see cref="Contentful.Core.Models.Management.ManagementToken"/> for a user.
        /// </summary>
        /// <param name="managementTokenId">The id of the management token to revoke.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The revoked <see cref="Contentful.Core.Models.Management.ManagementToken"/>.</returns>
        Task<ManagementToken> RevokeManagementToken(string managementTokenId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a single <see cref="Contentful.Core.Models.Management.User"/> for the currently logged in user.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.User"/>.</returns>
        Task<User> GetCurrentUser(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a collection of all <see cref="Contentful.Core.Models.Management.Organization"/> for a user.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.Organization"/>.</returns>
        Task<ContentfulCollection<Organization>> GetOrganizations(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a collection of all <see cref="Contentful.Core.Models.Management.ContentfulEnvironment"/> for a space.
        /// </summary>
        /// <param name="spaceId">The id of the space to get environments for. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.ContentfulEnvironment"/>.</returns>
        /// <exception cref="Contentful.Core.Errors.ContentfulException">There was an error when communicating with the Contentful API.</exception>
        Task<ContentfulCollection<ContentfulEnvironment>> GetEnvironments(string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a <see cref="Contentful.Core.Models.Management.ContentfulEnvironment"/> for a space.
        /// </summary>
        /// <param name="name">The name of the environment.</param>
        /// <param name="spaceId">The id of the space to create an environment in. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.ContentfulEnvironment"/>.</returns>
        /// <exception cref="System.ArgumentException">The required arguments were not provided.</exception>
        /// <exception cref="Contentful.Core.Errors.ContentfulException">There was an error when communicating with the Contentful API.</exception>
        Task<ContentfulEnvironment> CreateEnvironment(string name, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a <see cref="Contentful.Core.Models.Management.ContentfulEnvironment"/> for a space.
        /// </summary>
        /// <param name="id">The id of the environment to create.</param>
        /// <param name="name">The name of the environment.</param>
        /// <param name="version">The last known version of the environment. Must be set when updating an environment.</param>
        /// <param name="spaceId">The id of the space to create an environment in. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.ContentfulEnvironment"/>.</returns>
        /// <exception cref="System.ArgumentException">The required arguments were not provided.</exception>
        /// <exception cref="Contentful.Core.Errors.ContentfulException">There was an error when communicating with the Contentful API.</exception>
        Task<ContentfulEnvironment> CreateOrUpdateEnvironment(string id, string name, int? version = null, string spaceId = null, CancellationToken cancellationToken = default);

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
        Task<ContentfulEnvironment> CloneEnvironment(string id, string name, string sourceEnvironmentId, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a <see cref="Contentful.Core.Models.Management.ContentfulEnvironment"/> for a space.
        /// </summary>
        /// <param name="id">The id of the environment to get.</param>
        /// <param name="spaceId">The id of the space to get an environment in. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.ContentfulEnvironment"/>.</returns>
        /// <exception cref="System.ArgumentException">The required arguments were not provided.</exception>
        /// <exception cref="Contentful.Core.Errors.ContentfulException">There was an error when communicating with the Contentful API.</exception>
        Task<ContentfulEnvironment> GetEnvironment(string id, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a <see cref="Contentful.Core.Models.Management.ContentfulEnvironment"/> for a space.
        /// </summary>
        /// <param name="id">The id of the environment to delete.</param>
        /// <param name="spaceId">The id of the space to get an environment in. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.ContentfulEnvironment"/>.</returns>
        /// <exception cref="System.ArgumentException">The required arguments were not provided.</exception>
        /// <exception cref="Contentful.Core.Errors.ContentfulException">There was an error when communicating with the Contentful API.</exception>
        Task DeleteEnvironment(string id, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a collection of all <see cref="Contentful.Core.Models.Management.User"/> in a space.
        /// </summary>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.ApiKey"/>.</returns>
        /// <exception cref="Contentful.Core.Errors.ContentfulException">There was an error when communicating with the Contentful API.</exception>
        Task<ContentfulCollection<User>> GetAllUsers(string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a single <see cref="Contentful.Core.Models.Management.User"/> for a space.
        /// </summary>
        /// <param name="userId">The id of the user to get.</param>
        /// <param name="spaceId">The id of the space. Will default to the one set when creating the client.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.User"/>.</returns>
        /// <exception cref="System.ArgumentException">The <see name="spaceMembershipId">spaceMembershipId</see> parameter was null or empty.</exception>
        /// <exception cref="Contentful.Core.Errors.ContentfulException">There was an error when communicating with the Contentful API.</exception>
        Task<User> GetUser(string userId, string spaceId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a collection of <see cref="Contentful.Core.Models.Management.OrganizationMembership"/> for the specified organization.
        /// </summary>
        /// <param name="organizationId">The id of the organization.</param>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A collection of <see cref="Contentful.Core.Models.Management.OrganizationMembership"/>.</returns>
        Task<ContentfulCollection<OrganizationMembership>> GetOrganizationMemberships(string organizationId, string queryString = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a membership in an <see cref="Organization"/>.
        /// </summary>
        /// <param name="organizationId">The id of the organization to create a membership in.</param>
        /// <param name="role">The role the membership should have for that organization.</param>
        /// <param name="email">The email address of the membership.</param>
        /// <param name="suppressInvitation">Whether or not to suppress the invitation email.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The created <see cref="Contentful.Core.Models.Management.OrganizationMembership"/>.</returns>
        /// <exception cref="Contentful.Core.Errors.ContentfulException">There was an error when communicating with the Contentful API.</exception>
        Task<OrganizationMembership> CreateOrganizationMembership(string organizationId, string role, string email, bool suppressInvitation, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a single <see cref="Contentful.Core.Models.Management.OrganizationMembership"/> for a space.
        /// </summary>
        /// <param name="membershipId">The id of the membership to get.</param>
        /// <param name="organizationId">The id of the organization.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.SpaceMembership"/>.</returns>
        /// <exception cref="System.ArgumentException">The <see name="spaceMembershipId">spaceMembershipId</see> parameter was null or empty.</exception>
        /// <exception cref="Contentful.Core.Errors.ContentfulException">There was an error when communicating with the Contentful API.</exception>
        Task<OrganizationMembership> GetOrganizationMembership(string membershipId, string organizationId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a <see cref="Contentful.Core.Models.Management.OrganizationMembership"/> for a space.
        /// </summary>
        /// <param name="role">The role to set for the membership.</param>
        /// <param name="membershipId">The id of the membership to update.</param>
        /// <param name="organizationId">The id of the organization.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>The <see cref="Contentful.Core.Models.Management.OrganizationMembership"/>.</returns>
        /// <exception cref="Contentful.Core.Errors.ContentfulException">There was an error when communicating with the Contentful API.</exception>
        Task<OrganizationMembership> UpdateOrganizationMembership(string role, string membershipId, string organizationId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a <see cref="Contentful.Core.Models.Management.OrganizationMembership"/> for a space.
        /// </summary>
        /// <param name="membershipId">The id of the organization membership to delete.</param>
        /// <param name="organizationId">The id of the organization.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <exception cref="System.ArgumentException">The <see name="membershipId">membershipId</see> parameter was null or empty.</exception>
        /// <exception cref="Contentful.Core.Errors.ContentfulException">There was an error when communicating with the Contentful API.</exception>
        Task DeleteOrganizationMembership(string membershipId, string organizationId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a collection of all <see cref="Contentful.Core.Models.Management.UsagePeriod"/> for an organization.
        /// </summary>
        /// <param name="organizationId">The id of the organization to get usage periods for.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.UsagePeriod"/>.</returns>
        /// <exception cref="Contentful.Core.Errors.ContentfulException">There was an error when communicating with the Contentful API.</exception>
        Task<ContentfulCollection<UsagePeriod>> GetUsagePeriods(string organizationId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a collection of <see cref="Contentful.Core.Models.Management.ApiUsage"/> for an organization.
        /// </summary>
        /// <param name="organizationId">The id of the organization to get usage for.</param>
        /// <param name="type">The type of resource to get usage for, organization or space.</param>
        /// <param name="queryString">The optional querystring to add additional filtering to the query.</param>
        /// <param name="cancellationToken">The optional cancellation token to cancel the operation.</param>
        /// <returns>A <see cref="ContentfulCollection{T}"/> of <see cref="Contentful.Core.Models.Management.ApiUsage"/>.</returns>
        /// <exception cref="Contentful.Core.Errors.ContentfulException">There was an error when communicating with the Contentful API.</exception>
        Task<ContentfulCollection<ApiUsage>> GetResourceUsage(string organizationId, string type, string queryString = null, CancellationToken cancellationToken = default);
    }
}