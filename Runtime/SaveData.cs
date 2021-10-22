using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.GameBackend.CloudSave.Apis.Data;
using Unity.Services.Authentication.Internal;
using Unity.Services.CloudSave.Exceptions;
using UnityEngine;

namespace Unity.Services.CloudSave
{
    public static class SaveData
    {
        static ISaveDataClient _saveData;

        internal static void InitializeSaveData(IPlayerId playerId, IAccessToken accessToken, IDataApiClient cloudSaveDataApiClient)
        {
            IAuthentication authentication = new AuthenticationWrapper(playerId, accessToken);
            IApiClient apiClient = new ApiClient(Application.cloudProjectId, authentication, cloudSaveDataApiClient);
            _saveData = new SaveDataClient(apiClient, new CloudSaveApiErrorHandler());
        }

        /// <summary>
        /// Returns all keys stored in Cloud Save for the logged in player.
        /// Throws a CloudSaveException with a reason code and explanation of what happened.
        ///
        /// This method includes pagination.
        /// </summary>
        /// <returns>A list of keys stored in the server for the logged in player.</returns>
        public static async Task<List<string>> RetrieveAllKeysAsync()
        {
            return await _saveData.RetrieveAllKeysAsync();
        }

        /// <summary>
        /// Force upload one or more key-value pairs to the Cloud Save service - it ignores write lock validation.
        /// Throws a CloudSaveException with a reason code and explanation of what happened.
        /// 
        /// <code>Dictionary</code> as a parameter ensures the uniqueness of given keys.
        /// There is no client validation in place, which means the API can be called regardless if data is incorrect and/or missing.
        /// </summary>
        /// <param name="data">The dictionary of keys and corresponding values to upload</param>
        public static async Task ForceSaveAsync(Dictionary<string, object> data)
        {
            await _saveData.ForceSaveAsync(data);
        }

        /// <summary>
        /// Removes one key at the time. If a given key doesn't exist, there is no feedback in place to inform a developer about it. 
        /// There is no write lock or client validation implemented for this method.
        /// Throws a CloudSaveException with a reason code and explanation of what happened.
        /// 
        /// </summary>
        /// <param name="key">The key to be removed from the server</param>
        public static async Task ForceDeleteAsync(string key)
        {
            await _saveData.ForceDeleteAsync(key);
        }

        /// <summary>
        /// Downloads one or more values from Cloud Save, based on provided keys.
        /// <code>HashSet</code> as a parameter ensures the uniqueness of keys.
        /// There is no client validation in place.
        /// This method includes pagination.
        /// Throws a CloudSaveException with a reason code and explanation of what happened.
        /// </summary>
        /// <param name="keys">The HashSet of keys to download from the server</param>
        /// <returns>The dictionary of key-value pairs that represents the current state of data on the server</returns>
        public static async Task<Dictionary<string, string>> LoadAsync(HashSet<string> keys)
        {
            return await _saveData.LoadAsync(keys);
        }
        
        /// <summary>
        /// Downloads all data from Cloud Save.
        /// There is no client validation in place.
        /// This method includes pagination.
        /// Throws a CloudSaveException with a reason code and explanation of what happened.
        /// </summary>
        /// <returns>The dictionary of all key-value pairs that represents the current state of data on the server</returns>
        public static async Task<Dictionary<string, string>> LoadAllAsync()
        {
            return await _saveData.LoadAsync();
        }
    }
}