using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Services.CloudSave.Internal.Models;
using Unity.Services.CloudSave.Internal.Data;
using Unity.Services.CloudSave.Models;
using Unity.Services.Core;
using Unity.Services.Core.Configuration.Internal;

[assembly: InternalsVisibleTo("Unity.Services.CloudSave.Tests")]

namespace Unity.Services.CloudSave.Internal
{
    /// <summary>
    /// Wraps the generated API client to automatically pass the project ID and player ID into the requests.
    /// </summary>
    interface IPlayerDataApiClient
    {
        Task<Response<GetKeysResponse>> ListKeysAsync(string afterKey);
        Task<Response<GetItemsResponse>> LoadAsync(ISet<string> keys, string afterKey);
        Task<Response<SetItemBatchResponse>> SaveAsync(IDictionary<string, Unity.Services.CloudSave.Models.SaveItem> data);
        Task<Response<SetItemBatchResponse>> ForceSaveAsync(IDictionary<string, object> data);
        Task<Response> DeleteAsync(string key, string writeLock = null);
        Task<Response> DeleteAllAsync();
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

        public async Task<Response<GetKeysResponse>> ListKeysAsync(string afterKey)
        {
            ValidateRequiredDependencies();
            var request = new GetKeysRequest(m_CloudProjectId.GetCloudProjectId(), m_Authentication.GetPlayerId(), afterKey);

            return await m_DataClient.GetKeysAsync(request);
        }

        public async Task<Response<SetItemBatchResponse>> SaveAsync(IDictionary<string, Unity.Services.CloudSave.Models.SaveItem> data)
        {
            ValidateRequiredDependencies();
            var itemsList = new List<SetItemBody>();
            foreach (var item in data)
            {
                itemsList.Add(new SetItemBody(item.Key, item.Value.Value, item.Value.WriteLock));
            }
            var items = new SetItemBatchBody(itemsList);
            var request = new SetItemBatchRequest(m_CloudProjectId.GetCloudProjectId(), m_Authentication.GetPlayerId(), items);

            return await m_DataClient.SetItemBatchAsync(request);
        }

        public async Task<Response<SetItemBatchResponse>> ForceSaveAsync(IDictionary<string, object> data)
        {
            var saveItemData = data.ToDictionary(kv => kv.Key, kv => new Unity.Services.CloudSave.Models.SaveItem(kv.Value, null));
            return await SaveAsync(saveItemData);
        }

        public async Task<Response> DeleteAsync(string key, string writeLock = null)
        {
            ValidateRequiredDependencies();
            var request = new DeleteItemRequest(key, m_CloudProjectId.GetCloudProjectId(), m_Authentication.GetPlayerId(), writeLock);

            return await m_DataClient.DeleteItemAsync(request);
        }

        public async Task<Response> DeleteAllAsync()
        {
            ValidateRequiredDependencies();
            var request = new DeleteItemsRequest(m_CloudProjectId.GetCloudProjectId(), m_Authentication.GetPlayerId());

            return await m_DataClient.DeleteItemsAsync(request);
        }

        public async Task<Response<GetItemsResponse>> LoadAsync(ISet<string> keys, string afterKey)
        {
            ValidateRequiredDependencies();
            var requestedKeys = keys == null ? null : new List<string>(keys);

            var request = new GetItemsRequest(m_CloudProjectId.GetCloudProjectId(), m_Authentication.GetPlayerId(), requestedKeys, afterKey);

            return await m_DataClient.GetItemsAsync(request);
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
    }
}
