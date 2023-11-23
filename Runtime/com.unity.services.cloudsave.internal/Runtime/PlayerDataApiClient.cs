using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Services.CloudSave.Internal.Models;
using Unity.Services.CloudSave.Internal.Data;
using Unity.Services.CloudSave.Models;
using Unity.Services.Core;
using Unity.Services.Core.Configuration.Internal;
using FieldFilter = Unity.Services.CloudSave.Internal.Models.FieldFilter;

[assembly: InternalsVisibleTo("Unity.Services.CloudSave.Tests")]

namespace Unity.Services.CloudSave.Internal
{
    /// <summary>
    /// Wraps the generated API client to automatically pass the project ID and player ID into the requests.
    /// </summary>
    interface IPlayerDataApiClient
    {
        Task<Response<GetKeysResponse>> ListKeysAsync(string afterKey, AccessClass accessClass = AccessClass.Default, string playerId = null);
        Task<Response<GetItemsResponse>> LoadAsync(ISet<string> keys, string afterKey, AccessClass accessClass = AccessClass.Default, string playerId = null);
        Task<Response<SetItemBatchResponse>> SaveAsync(IDictionary<string, Unity.Services.CloudSave.Models.SaveItem> data, AccessClass accessClass = AccessClass.Default);
        Task<Response<SetItemBatchResponse>> ForceSaveAsync(IDictionary<string, object> data, AccessClass accessClass = AccessClass.Default);
        Task<Response> DeleteAsync(string key, string writeLock = null, AccessClass accessClass = AccessClass.Default);
        Task<Response> DeleteAllAsync(AccessClass accessClass = AccessClass.Default);
        Task<Response<QueryIndexResponse>> QueryAsync(Query query, AccessClass accessClass = AccessClass.Public);
    }

    class PlayerDataApiClient : IPlayerDataApiClient
    {
        readonly ICloudProjectId m_CloudProjectId;
        readonly Internal.Apis.Data.IDataApiClient m_DataClient;
        readonly IAuthentication m_Authentication;

        internal PlayerDataApiClient(ICloudProjectId cloudProjectId, IAuthentication authentication, Internal.Apis.Data.IDataApiClient dataClient)
        {
            m_CloudProjectId = cloudProjectId;
            m_DataClient = dataClient;
            m_Authentication = authentication;
        }

        public async Task<Response<GetKeysResponse>> ListKeysAsync(string afterKey, AccessClass accessClass = AccessClass.Default, string playerId = null)
        {
            ValidateRequiredDependencies();

            switch (accessClass)
            {
                case AccessClass.Default:
                    var request = new GetKeysRequest(m_CloudProjectId.GetCloudProjectId(), m_Authentication.GetPlayerId(), afterKey);
                    return await m_DataClient.GetKeysAsync(request);
                case AccessClass.Protected:
                    var protectedRequest = new GetProtectedKeysRequest(m_CloudProjectId.GetCloudProjectId(), m_Authentication.GetPlayerId(), afterKey);
                    return await m_DataClient.GetProtectedKeysAsync(protectedRequest);
                case AccessClass.Public:
                    playerId ??= m_Authentication.GetPlayerId();
                    var publicRequest = new GetPublicKeysRequest(m_CloudProjectId.GetCloudProjectId(), playerId, afterKey);
                    return await m_DataClient.GetPublicKeysAsync(publicRequest);
                case AccessClass.Private:
                default:
                    throw new InvalidOperationException(
                        "ListKeysAsync can only be called with Default, Protected, or Public AccessClass");
            }
        }

        public async Task<Response<SetItemBatchResponse>> SaveAsync(IDictionary<string, Unity.Services.CloudSave.Models.SaveItem> data, AccessClass accessClass = AccessClass.Default)
        {
            ValidateRequiredDependencies();
            var itemsList = new List<SetItemBody>();
            foreach (var item in data)
            {
                itemsList.Add(new SetItemBody(item.Key, item.Value.Value, item.Value.WriteLock));
            }
            var items = new SetItemBatchBody(itemsList);

            switch (accessClass)
            {
                case AccessClass.Default:
                    var request = new SetItemBatchRequest(m_CloudProjectId.GetCloudProjectId(), m_Authentication.GetPlayerId(), items);
                    return await m_DataClient.SetItemBatchAsync(request);
                case AccessClass.Public:
                    var publicRequest = new SetPublicItemBatchRequest(m_CloudProjectId.GetCloudProjectId(), m_Authentication.GetPlayerId(), items);
                    return await m_DataClient.SetPublicItemBatchAsync(publicRequest);
                case AccessClass.Private:
                case AccessClass.Protected:
                default:
                    throw new InvalidOperationException(
                        "SaveAsync can only be called with Default or Public AccessClass");
            }
        }

        public async Task<Response<SetItemBatchResponse>> ForceSaveAsync(IDictionary<string, object> data, AccessClass accessClass = AccessClass.Default)
        {
            var saveItemData = data.ToDictionary(kv => kv.Key, kv => new Unity.Services.CloudSave.Models.SaveItem(kv.Value, null));
            return await SaveAsync(saveItemData, accessClass);
        }

        public async Task<Response> DeleteAsync(string key, string writeLock = null, AccessClass accessClass = AccessClass.Default)
        {
            ValidateRequiredDependencies();

            switch (accessClass)
            {
                case AccessClass.Default:
                    var request = new DeleteItemRequest(key, m_CloudProjectId.GetCloudProjectId(), m_Authentication.GetPlayerId(), writeLock);
                    return await m_DataClient.DeleteItemAsync(request);
                case AccessClass.Public:
                    var publicRequest = new DeletePublicItemRequest(key, m_CloudProjectId.GetCloudProjectId(), m_Authentication.GetPlayerId(), writeLock);
                    return await m_DataClient.DeletePublicItemAsync(publicRequest);
                case AccessClass.Private:
                case AccessClass.Protected:
                default:
                    throw new InvalidOperationException(
                        "DeleteAsync can only be called with Default or Public AccessClass");
            }
        }

        public async Task<Response> DeleteAllAsync(AccessClass accessClass = AccessClass.Default)
        {
            ValidateRequiredDependencies();

            switch (accessClass)
            {
                case AccessClass.Default:
                    var request = new DeleteItemsRequest(m_CloudProjectId.GetCloudProjectId(), m_Authentication.GetPlayerId());
                    return await m_DataClient.DeleteItemsAsync(request);
                case AccessClass.Public:
                    var publicRequest = new DeletePublicItemsRequest(m_CloudProjectId.GetCloudProjectId(), m_Authentication.GetPlayerId());
                    return await m_DataClient.DeletePublicItemsAsync(publicRequest);
                case AccessClass.Private:
                case AccessClass.Protected:
                default:
                    throw new InvalidOperationException(
                        "DeleteAllAsync can only be called with Default or Public AccessClass");
            }
        }

        public async Task<Response<GetItemsResponse>> LoadAsync(ISet<string> keys, string afterKey, AccessClass accessClass = AccessClass.Default, string playerId = null)
        {
            ValidateRequiredDependencies();
            var requestedKeys = keys == null ? null : new List<string>(keys);

            switch (accessClass)
            {
                case AccessClass.Default:
                    var request = new GetItemsRequest(m_CloudProjectId.GetCloudProjectId(), m_Authentication.GetPlayerId(), requestedKeys, afterKey);
                    return await m_DataClient.GetItemsAsync(request);
                case AccessClass.Protected:
                    var protectedRequest = new GetProtectedItemsRequest(m_CloudProjectId.GetCloudProjectId(), m_Authentication.GetPlayerId(), requestedKeys, afterKey);
                    return await m_DataClient.GetProtectedItemsAsync(protectedRequest);
                case AccessClass.Public:
                    playerId ??= m_Authentication.GetPlayerId();
                    var publicRequest = new GetPublicItemsRequest(m_CloudProjectId.GetCloudProjectId(), playerId, requestedKeys, afterKey);
                    return await m_DataClient.GetPublicItemsAsync(publicRequest);
                case AccessClass.Private:
                default:
                    throw new InvalidOperationException(
                        "LoadAsync can only be called with Default, Protected, or Public AccessClass");
            }
        }

        public async Task<Response<QueryIndexResponse>> QueryAsync(Query query,
            AccessClass accessClass = AccessClass.Public)
        {
            ValidateRequiredDependencies();
            var queryFields = query?.Fields?.Select(FieldFilterToInternalFieldFilter).ToList() ??
                new List<FieldFilter>();
            var queryKeys = query?.ReturnKeys?.ToList() ?? new List<string>();
            var offset = query?.Offset ?? 0;
            var limit = query?.Limit ?? 0;

            switch (accessClass)
            {
                case AccessClass.Public:
                    var request = new QueryPublicPlayerDataRequest(m_CloudProjectId.GetCloudProjectId(),
                        new QueryIndexBody(queryFields, queryKeys, offset, limit));
                    return await m_DataClient.QueryPublicPlayerDataAsync(request);
                case AccessClass.Default:
                case AccessClass.Private:
                case AccessClass.Protected:
                default:
                    throw new InvalidOperationException(
                        "QueryAsync can only be called with Public AccessClass");
            }
        }

        void ValidateRequiredDependencies()
        {
            if (string.IsNullOrEmpty(m_CloudProjectId.GetCloudProjectId()))
            {
                throw new CloudSaveException(CloudSaveExceptionReason.ProjectIdMissing, CommonErrorCodes.Unknown,
                    "Project ID is missing - make sure the project is correctly linked to your game and try again.", null);
            }

            if (string.IsNullOrEmpty(m_Authentication.GetPlayerId()))
            {
                throw new CloudSaveException(CloudSaveExceptionReason.PlayerIdMissing, CommonErrorCodes.Unknown,
                    "Player ID is missing - ensure you are signed in through the Authentication SDK and try again.", null);
            }

            if (string.IsNullOrEmpty(m_Authentication.GetAccessToken()))
            {
                throw new CloudSaveException(CloudSaveExceptionReason.AccessTokenMissing, CommonErrorCodes.InvalidToken,
                    "Access token is missing - ensure you are signed in through the Authentication SDK and try again.", null);
            }
        }

        static FieldFilter FieldFilterToInternalFieldFilter(CloudSave.Models.FieldFilter fieldFilter)
        {
            return new FieldFilter(fieldFilter.Key, fieldFilter.Value, (FieldFilter.OpOptions)fieldFilter.Op,
                fieldFilter.Asc);
        }
    }
}
