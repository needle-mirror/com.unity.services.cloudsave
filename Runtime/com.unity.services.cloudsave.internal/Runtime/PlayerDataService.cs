using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.CloudSave.Internal.Models;
using Unity.Services.CloudSave.Models;
using Item = Unity.Services.CloudSave.Models.Item;
using ApiItem = Unity.Services.CloudSave.Internal.Models.Item;

namespace Unity.Services.CloudSave.Internal
{
    public interface IPlayerDataService
    {
        /// <summary>
        /// Returns all keys stored in Cloud Save for the logged in player.
        /// Throws a CloudSaveException with a reason code and explanation of what happened.
        /// </summary>
        /// <returns>A list of keys and their metadata as stored in the server for the logged in player.</returns>
        /// <exception cref="CloudSaveException">Thrown if request is unsuccessful.</exception>
        /// <exception cref="CloudSaveValidationException">Thrown if the service returned validation error.</exception>
        /// <exception cref="CloudSaveRateLimitedException">Thrown if the service returned rate limited error.</exception>
        Task<List<ItemKey>> ListAllKeysAsync();

        /// <summary>
        /// Downloads data from Cloud Save for the keys provided.
        /// There is no client validation in place for the provided keys.
        /// Throws a CloudSaveException with a reason code and explanation of what happened.
        /// </summary>
        /// <param name="keys">The optional set of keys to load data for</param>
        /// <returns>The dictionary of all key-value pairs that represents the current state of data on the server including their write locks</returns>
        /// <exception cref="CloudSaveException">Thrown if request is unsuccessful.</exception>
        /// <exception cref="CloudSaveValidationException">Thrown if the service returned validation error.</exception>
        /// <exception cref="CloudSaveRateLimitedException">Thrown if the service returned rate limited error.</exception>
        Task<Dictionary<string, Item>> LoadAsync(ISet<string> keys);

        /// <summary>
        /// Downloads data from Cloud Save for all keys.
        /// Throws a CloudSaveException with a reason code and explanation of what happened.
        /// </summary>
        /// <returns>The dictionary of all key-value pairs that represents the current state of data on the server including their write locks</returns>
        /// <exception cref="CloudSaveException">Thrown if request is unsuccessful.</exception>
        /// <exception cref="CloudSaveValidationException">Thrown if the service returned validation error.</exception>
        /// <exception cref="CloudSaveRateLimitedException">Thrown if the service returned rate limited error.</exception>
        Task<Dictionary<string, Item>> LoadAllAsync();

        /// <summary>
        /// Upload one or more key-value pairs to the Cloud Save service, with optional write lock validation.
        /// If a write lock is provided on an item and it does not match with the existing write lock, will throw a conflict exception.
        /// If the write lock for an item is set to null, the write lock validation for that item will be skipped and any existing value
        /// currently stored for that key will be overwritten.
        /// Keys can only contain alphanumeric characters, dashes, and underscores and be up to a length of 255 characters.
        /// Throws a CloudSaveException with a reason code and explanation of what happened.
        ///
        /// <code>Dictionary</code> as a parameter ensures the uniqueness of given keys.
        /// There is no client validation in place, which means the API can be called regardless if data or keys are incorrect, invalid, and/or missing.
        /// </summary>
        /// <param name="data">The dictionary of keys and corresponding values to upload, together with optional write lock to check conflict</param>
        /// <returns>The dictionary of saved keys and the corresponding updated write lock</returns>
        /// <exception cref="CloudSaveException">Thrown if request is unsuccessful.</exception>
        /// <exception cref="CloudSaveValidationException">Thrown if the service returned validation error.</exception>
        /// <exception cref="CloudSaveRateLimitedException">Thrown if the service returned rate limited error.</exception>
        /// <exception cref="CloudSaveConflictException">Thrown if the service returned write lock conflict error.</exception>
        Task<Dictionary<string, string>> SaveAsync(IDictionary<string, SaveItem> data);

        /// <summary>
        /// Upload one or more key-value pairs to the Cloud Save service without write lock validation, overwriting any values
        /// that are currently stored under the given keys.
        /// Key can only contain alphanumeric characters, dashes, and underscores and be up to a length of 255 characters.
        /// Throws a CloudSaveException with a reason code and explanation of what happened.
        ///
        /// <code>Dictionary</code> as a parameter ensures the uniqueness of given keys.
        /// There is no client validation in place, which means the API can be called regardless if data is incorrect, invalid, and/or missing.
        /// </summary>
        /// <param name="data">The dictionary of keys and corresponding values to upload</param>
        /// <returns>The dictionary of saved keys and the corresponding updated write lock</returns>
        /// <exception cref="CloudSaveException">Thrown if request is unsuccessful.</exception>
        /// <exception cref="CloudSaveValidationException">Thrown if the service returned validation error.</exception>
        /// <exception cref="CloudSaveRateLimitedException">Thrown if the service returned rate limited error.</exception>
        Task<Dictionary<string, string>> SaveAsync(IDictionary<string, object> data);

        /// <summary>
        /// Removes one key at a time, with optional write lock validation. If the given key doesn't exist, there is no feedback in place to inform a developer about it.
        /// If a write lock is provided and it does not match with the existing write lock, will throw a conflict exception.
        /// There is no client validation on the arguments for this method.
        /// Throws a CloudSaveException with a reason code and explanation of what happened.
        ///
        /// </summary>
        /// <param name="key">The key to be removed from the server</param>
        /// <param name="deleteOptions">The optional options object for specifying the write lock to check conflict in the server</param>
        /// <exception cref="CloudSaveException">Thrown if request is unsuccessful.</exception>
        /// <exception cref="CloudSaveValidationException">Thrown if the service returned validation error.</exception>
        /// <exception cref="CloudSaveRateLimitedException">Thrown if the service returned rate limited error.</exception>
        /// <exception cref="CloudSaveConflictException">Thrown if the service returned write lock conflict error.</exception>
        Task DeleteAsync(string key, DeleteOptions deleteOptions = null);

        /// <summary>
        /// Removes all keys for the player without write lock validation.
        /// Throws a CloudSaveException with a reason code and explanation of what happened.
        ///
        /// </summary>
        /// <exception cref="CloudSaveException">Thrown if request is unsuccessful.</exception>
        /// <exception cref="CloudSaveRateLimitedException">Thrown if the service returned rate limited error.</exception>
        Task DeleteAllAsync();
    }

    class PlayerDataService : IPlayerDataService
    {
        readonly IPlayerDataApiClient m_PlayerDataApiClient;
        readonly IApiErrorHandler m_ErrorHandler;

        internal PlayerDataService(IPlayerDataApiClient playerDataClient, IApiErrorHandler errorHandler)
        {
            m_PlayerDataApiClient = playerDataClient;
            m_ErrorHandler = errorHandler;
        }

        public async Task<List<ItemKey>> ListAllKeysAsync()
        {
            return await m_ErrorHandler.RunWithErrorHandling(async() =>
            {
                var returnSet = new List<ItemKey>();
                Response<GetKeysResponse> response;
                string lastAddedKey = null;
                do
                {
                    response = await m_PlayerDataApiClient.ListKeysAsync(lastAddedKey);
                    var items = response.Result.Results;
                    if (items.Count > 0)
                    {
                        foreach (var item in items)
                        {
                            returnSet.Add(new ItemKey(item.Key, item.WriteLock, item.Modified?.Date));
                        }

                        lastAddedKey = items[items.Count - 1].Key;
                    }
                }
                while (!string.IsNullOrEmpty(response.Result.Links.Next));

                return returnSet;
            });
        }

        public async Task<Dictionary<string, Item>> LoadAsync(ISet<string> keys)
        {
            if (keys == null || keys.Count == 0)
            {
                return new Dictionary<string, Item>();
            }

            return await LoadWithErrorHandlingAsync(keys);
        }

        public async Task<Dictionary<string, Item>> LoadAllAsync()
        {
            return await LoadWithErrorHandlingAsync();
        }

        async Task<Dictionary<string, Item>> LoadWithErrorHandlingAsync(ISet<string> keys = null)
        {
            return await m_ErrorHandler.RunWithErrorHandling(async() =>
            {
                var result = new Dictionary<string, Item>();
                Response<GetItemsResponse> response;
                string lastAddedKey = null;
                do
                {
                    response = await m_PlayerDataApiClient.LoadAsync(keys, lastAddedKey);
                    var items = response.Result.Results;
                    if (items.Count > 0)
                    {
                        foreach (var item in items)
                        {
                            result[item.Key] = new Item(item);
                        }

                        lastAddedKey = items[items.Count - 1].Key;
                    }
                }
                while (!string.IsNullOrEmpty(response.Result.Links.Next));

                return result;
            });
        }

        public async Task<Dictionary<string, string>> SaveAsync(IDictionary<string, SaveItem> data)
        {
            if (data == null || data.Count == 0)
            {
                return new Dictionary<string, string>();
            }

            return await SaveWithErrorHandlingAsync(data);
        }

        public async Task<Dictionary<string, string>> SaveAsync(IDictionary<string, object> data)
        {
            if (data == null || data.Count == 0)
            {
                return new Dictionary<string, string>();
            }

            var dict = new Dictionary<string, SaveItem>();
            foreach (var item in data)
            {
                dict.Add(item.Key, new SaveItem(item.Value, null));
            }

            return await SaveWithErrorHandlingAsync(dict);
        }

        async Task<Dictionary<string, string>> SaveWithErrorHandlingAsync(IDictionary<string, SaveItem> data)
        {
            return await m_ErrorHandler.RunWithErrorHandling(async() =>
            {
                if (data.Count < 20)
                {
                    var response = await m_PlayerDataApiClient.SaveAsync(data);
                    return response.Result.Results.ToDictionary(r => r.Key, r => r.WriteLock);
                }

                var results = new Dictionary<string, string>();
                var batches = Convert.ToInt32(Math.Ceiling(data.Count / 20.0f));
                for (var i = 0; i < batches; i++)
                {
                    var batch = data.Skip(i * 20).Take(20)
                        .ToDictionary(k => k.Key, v => v.Value);
                    var response = await m_PlayerDataApiClient.SaveAsync(batch);
                    response.Result.Results.ForEach(item => results.Add(item.Key, item.WriteLock));
                }

                return results;
            });
        }

        public async Task DeleteAsync(string key, DeleteOptions deleteOptions = null)
        {
            await m_ErrorHandler.RunWithErrorHandling(async() =>
            {
                await m_PlayerDataApiClient.DeleteAsync(key, deleteOptions?.WriteLock);
            });
        }

        public async Task DeleteAllAsync()
        {
            await m_ErrorHandler.RunWithErrorHandling(async() =>
            {
                await m_PlayerDataApiClient.DeleteAllAsync();
            });
        }
    }
}
