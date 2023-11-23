/*
 * Not currently deprecated as the behavior is still supported and expected, but the methods have been superseded by
 * the new methods with appropriate Options objects in PlayerDataService.cs, which provide additional functionality
 * (AccessClass and CancellationToken support). In future we could deprecate/remove these methods if we are making
 * another breaking change to the SDK, so that we bundle our breaking changes in one go.
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave.Models;
using Unity.Services.CloudSave.Models.Data.Player;
using Item = Unity.Services.CloudSave.Models.Item;
using ApiItem = Unity.Services.CloudSave.Internal.Models.Item;

namespace Unity.Services.CloudSave.Internal
{
    public partial interface IPlayerDataService
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
        [Obsolete("The interface provided by CloudSaveService.Instance.Data.Player.DeleteAsync(string key, Unity.Services.CloudSave.DeleteOptions deleteOptions)" +
                  " has been replaced by CloudSaveService.Instance.Data.Player.DeleteAsync(string key, Unity.Services.CloudSave.Models.Data.Player.DeleteOptions options)," +
                  " and should be accessed from there instead. This API will be removed in an upcoming release.", false)]
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

    partial class PlayerDataService
    {
        public async Task<List<ItemKey>> ListAllKeysAsync()
        {
            return await ListAllKeysAsync(new ListAllKeysOptions());
        }

        public async Task<Dictionary<string, Item>> LoadAsync(ISet<string> keys)
        {
            return await LoadAsync(keys, new LoadOptions());
        }

        public async Task<Dictionary<string, Item>> LoadAllAsync()
        {
            return await LoadAllAsync(new LoadAllOptions());
        }

        public async Task<Dictionary<string, string>> SaveAsync(IDictionary<string, SaveItem> data)
        {
            if (data == null || data.Count == 0)
            {
                return new Dictionary<string, string>();
            }

            return await SaveAsync(data, new CloudSave.Models.Data.Player.SaveOptions());
        }

        public async Task<Dictionary<string, string>> SaveAsync(IDictionary<string, object> data)
        {
            return await SaveAsync(data, new CloudSave.Models.Data.Player.SaveOptions());
        }

        public async Task DeleteAsync(string key, DeleteOptions deleteOptions = null)
        {
            await DeleteAsync(key,
                new CloudSave.Models.Data.Player.DeleteOptions { WriteLock = deleteOptions?.WriteLock });
        }

        public async Task DeleteAllAsync()
        {
            await DeleteAllAsync(new DeleteAllOptions());
        }
    }
}
