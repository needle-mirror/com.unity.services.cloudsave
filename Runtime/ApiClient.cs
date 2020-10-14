using System.Collections.Generic;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Services.CloudSave.Internal;
using Unity.Services.CloudSave.Internal.Apis.Data;
using Unity.Services.CloudSave.Internal.Data;
using Unity.Services.CloudSave.Internal.Models;
using Unity.Services.Core;

[assembly: InternalsVisibleTo("Unity.Services.CloudSave.Tests")]

namespace Unity.Services.CloudSave
{
    internal interface IApiClient
    {
        Task<Response<GetKeysResponse>> RetrieveKeysAsync(string afterKey);
        Task<Response<SetItemBatchResponse>> ForceSaveAsync(Dictionary<string, object> data);
        Task<Response> ForceDeleteAsync(string key);
        Task<Response<GetItemsResponse>> LoadAsync(HashSet<string> keys, string afterKey);
    }

    internal class ApiClient: IApiClient
    {
        readonly string m_CloudProjectId;
        readonly IDataApiClient m_CloudSaveClient;
        readonly IAuthentication m_Authentication;

        internal ApiClient(string cloudProjectId, IAuthentication authentication, IDataApiClient cloudSaveClient)
        {
            m_CloudProjectId = cloudProjectId;
            m_CloudSaveClient = cloudSaveClient;
            m_Authentication = authentication;
        }

        public async Task<Response<GetKeysResponse>> RetrieveKeysAsync(string afterKey)
        {
            ValidateRequiredDependencies();
            GetKeysRequest request = new GetKeysRequest(m_CloudProjectId, m_Authentication.GetPlayerId(), afterKey);
            
            return await m_CloudSaveClient.GetKeysAsync(request);
        }
        
        public async Task<Response<SetItemBatchResponse>> ForceSaveAsync(Dictionary<string, object> data)
        {
            ValidateRequiredDependencies();
            List<SetItemBody> itemsList = new List<SetItemBody>();
            foreach (var item in data)
            {
                itemsList.Add(new SetItemBody(item.Key, item.Value));
            }

            SetItemBatchBody items = new SetItemBatchBody(itemsList);
            SetItemBatchRequest request = new SetItemBatchRequest(m_CloudProjectId, m_Authentication.GetPlayerId(), items);

            return await m_CloudSaveClient.SetItemBatchAsync(request);
        }

        public async Task<Response> ForceDeleteAsync(string key)
        {
            ValidateRequiredDependencies();
            DeleteItemRequest request = new DeleteItemRequest(key, Application.cloudProjectId, m_Authentication.GetPlayerId());

            return await m_CloudSaveClient.DeleteItemAsync(request);
        }
        
        public async Task<Response<GetItemsResponse>> LoadAsync(HashSet<string> keys, string afterKey)
        {
            ValidateRequiredDependencies();
            List<string> requestedKeys = (null == keys) ? null : new List<string>(keys);
            
            GetItemsRequest request = new GetItemsRequest(m_CloudProjectId, m_Authentication.GetPlayerId(), requestedKeys, afterKey);
            
            return await m_CloudSaveClient.GetItemsAsync(request);
        }
        
        private void ValidateRequiredDependencies()
        {
            if (String.IsNullOrEmpty(m_CloudProjectId))
            {
                throw new CloudSaveException(CloudSaveExceptionReason.ProjectIdMissing, CommonErrorCodes.Unknown,
                    "Project ID is missing - make sure the project is correctly linked to your game and try again.", null);
            } 
            
            if (String.IsNullOrEmpty(m_Authentication.GetPlayerId()))
            {
                throw new CloudSaveException(CloudSaveExceptionReason.PlayerIdMissing, CommonErrorCodes.Unknown,
                    "Player ID is missing - ensure you are signed in through the Authentication SDK and try again.", null);
            } 
            
            if (String.IsNullOrEmpty(m_Authentication.GetAccessToken()))
            {
                throw new CloudSaveException(CloudSaveExceptionReason.AccessTokenMissing, CommonErrorCodes.InvalidToken,
                    "Access token is missing - ensure you are signed in through the Authentication SDK and try again.", null);
            }
        }
    }
}
