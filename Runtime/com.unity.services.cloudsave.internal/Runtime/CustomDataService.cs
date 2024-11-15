using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.CloudSave.Internal.Models;
using Unity.Services.CloudSave.Models;
using Unity.Services.CloudSave.Models.Data.Custom;
using Item = Unity.Services.CloudSave.Models.Item;

namespace Unity.Services.CloudSave.Internal
{
    /// <summary>
    /// Interface for Custom Data Service.
    /// </summary>
    public interface ICustomDataService
    {
        /// <summary>
        /// Returns all keys stored in Cloud Save for the specified custom data ID.
        /// Throws a CloudSaveException with a reason code and explanation of what happened.
        /// </summary>
        /// <param name="customDataID">The custom data ID to return all keys for.</param>
        /// <returns>A list of keys and their metadata as stored in the server for the logged in player.</returns>
        /// <exception cref="CloudSaveException">Thrown if request is unsuccessful.</exception>
        /// <exception cref="CloudSaveValidationException">Thrown if the service returned validation error.</exception>
        /// <exception cref="CloudSaveRateLimitedException">Thrown if the service returned rate limited error.</exception>
        Task<List<ItemKey>> ListAllKeysAsync(string customDataID);

        /// <summary>
        /// Downloads items from Cloud Save for the custom data ID and keys provided.
        /// There is no client validation in place.
        /// Throws a CloudSaveException with a reason code and explanation of what happened.
        /// </summary>
        /// <param name="customDataID">The custom data ID to return keys for.</param>
        /// <param name="keys">The keys to return.</param>
        /// <returns>The dictionary of all key-value pairs that represents the current state of data on the server</returns>
        /// <exception cref="CloudSaveException">Thrown if request is unsuccessful.</exception>
        /// <exception cref="CloudSaveValidationException">Thrown if the service returned validation error.</exception>
        /// <exception cref="CloudSaveRateLimitedException">Thrown if the service returned rate limited error.</exception>
        Task<Dictionary<string, Item>> LoadAsync(string customDataID, ISet<string> keys);

        /// <summary>
        /// Downloads all items from Cloud Save for the custom data ID.
        /// There is no client validation in place.
        /// Throws a CloudSaveException with a reason code and explanation of what happened.
        /// </summary>
        /// <param name="customDataID">The custom data ID to return keys for.</param>
        /// <returns>The dictionary of all key-value pairs that represents the current state of data on the server</returns>
        /// <exception cref="CloudSaveException">Thrown if request is unsuccessful.</exception>
        /// <exception cref="CloudSaveValidationException">Thrown if the service returned validation error.</exception>
        /// <exception cref="CloudSaveRateLimitedException">Thrown if the service returned rate limited error.</exception>
        Task<Dictionary<string, Item>> LoadAllAsync(string customDataID);

        /// <summary>
        /// Queries indexed custom data from Cloud Save, and returns the requested keys for matching items.
        /// Throws a CloudSaveException with a reason code and explanation of what happened.
        /// </summary>
        /// <param name="query">The query conditions to apply, including field filters and sort orders</param>
        /// <param name="options">Options to modify the behavior of the method</param>
        /// <returns>The dictionary of all key-value pairs that represents the current state of data on the server including their write locks</returns>
        /// <exception cref="CloudSaveException">Thrown if request is unsuccessful.</exception>
        /// <exception cref="CloudSaveValidationException">Thrown if the service returned validation error.</exception>
        /// <exception cref="CloudSaveRateLimitedException">Thrown if the service returned rate limited error.</exception>
        Task<List<EntityData>> QueryAsync(Query query, QueryOptions options = null);
    }

    class CustomDataService : ICustomDataService
    {
        readonly ICustomDataApiClient m_DataApiClient;
        readonly IApiErrorHandler m_ErrorHandler;

        internal CustomDataService(ICustomDataApiClient customDataClient, IApiErrorHandler errorHandler)
        {
            m_DataApiClient = customDataClient;
            m_ErrorHandler = errorHandler;
        }

        public async Task<List<ItemKey>> ListAllKeysAsync(string customID)
        {
            return await m_ErrorHandler.RunWithErrorHandling(async() =>
            {
                var returnSet = new List<ItemKey>();
                Response<GetKeysResponse> response;
                string lastAddedKey = null;
                do
                {
                    response = await m_DataApiClient.ListKeysAsync(customID, lastAddedKey);
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

        public async Task<Dictionary<string, Item>> LoadAsync(string customID, ISet<string> keys)
        {
            if (keys == null || keys.Count == 0)
            {
                return new Dictionary<string, Item>();
            }

            return await LoadWithErrorHandlingAsync(customID, keys);
        }

        public async Task<Dictionary<string, Item>> LoadAllAsync(string customID)
        {
            return await LoadWithErrorHandlingAsync(customID);
        }

        async Task<Dictionary<string, Item>> LoadWithErrorHandlingAsync(string customID, ISet<string> keys = null)
        {
            return await m_ErrorHandler.RunWithErrorHandling(async() =>
            {
                var result = new Dictionary<string, Item>();
                Response<GetItemsResponse> response;
                string lastAddedKey = null;
                do
                {
                    response = await m_DataApiClient.LoadAsync(customID, keys, lastAddedKey);
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

        public async Task<List<EntityData>> QueryAsync(Query query, QueryOptions options = null)
        {
            return await m_ErrorHandler.RunWithErrorHandling(async() =>
            {
                var queryResponse = await m_DataApiClient.QueryAsync(query);
                return queryResponse.Result.Results.Select(ed => new EntityData(ed.Id, ed.Data.Select(item => new Item(item)).ToList())).ToList();
            });
        }
    }
}
